using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Framework;
using Web.Models.Problem;
using Framework.Services;
using Data.Models;
using Data.Models.Enums;
using Data.S3;
using CommunityToolkit.HighPerformance.Helpers;
using System.Text;
using System.Web;
using Framework.Services.ProblemServices;
using Framework.Services.FileServices;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers;

[Authorize]
public class ProblemController : AcadnetController
{
    private readonly ProblemServiceFactory _problemServiceFactory;
    private readonly ICourseService _categoryService;
    private readonly IFileService _fileService;
    private readonly ICheckerService _checkerService;

    public ProblemController(
        ProblemServiceFactory problemServiceFactory,
        ICourseService categoryService,
        FileServiceFactory fileServiceFactory,
        ICheckerService checkerService
    )
    {
        _problemServiceFactory = problemServiceFactory;
        _categoryService = categoryService;
        _fileService = fileServiceFactory.GetFileService();
        _checkerService = checkerService;
    }

    [HttpGet]
    public IActionResult Create([FromQuery] int? categoryId)
    {
        if (categoryId == null)
        {
            AddError("Category not found!");
            return RedirectToAction("Index", "Course");
        }

        return View(new CreateProblemViewModel { CategoryId = categoryId });
    }

    [HttpPost]
    public IActionResult Create(CreateProblemViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var _category = _categoryService.GetCategory(model.CategoryId!.Value);

        if (_category == null)
        {
            AddError("Category not found!");
            return RedirectToAction("Index", "Course");
        }

        var _problem = new Problem
        {
            Name = model.Name,
            // problems are incomplete after creation, because they don't have any files
            Status = ProblemStatus.Incomplete,
            Category = _category,
            Type = model.ProblemType
        };

        var _problemService = _problemServiceFactory.GetServiceByType(_problem.Type);
        _problemService.CreateProblem(_problem);

        // get course to know where to return
        var _course = _categoryService.GetCourseByCategory(_category.Id);

        return RedirectToAction("Categories", "Course", new { courseId = _course!.Id, categoryParent = _category.Id });
    }

    [HttpGet]
    public IActionResult Solve([FromQuery] int? problemId = default!)
    {
        if (problemId == null)
        {
            AddError("Problem not found!");
            return RedirectToAction("Index", "Course");
        }

        var _problemService = _problemServiceFactory.GetServiceById(problemId.Value);
        var _problem = _problemService.GetProblem(problemId.Value);

        if (_problem == null || _problem.Status != ProblemStatus.Ready)
        {
            AddError("Problem not found!");
            return RedirectToAction("Index", "Course");
        }

        var _output = new SolveProblemViewModel
        {
            Id = _problem.Id,
            Name = _problem.Name,
            StatementHtml = _problemService.GetProblemStatementHtml(_problem),
            IsSolved = _problemService.HasSolvedProblem(_problem, SecurityContext.User!)
        };

        return View(_output);
    }

    [HttpPost]
    public async Task<IActionResult> UploadSubimission([FromQuery] int? problemId = default!)
    {
        if (problemId == null)
        {
            AddError("Problem not found!");
            return RedirectToAction("Index", "Course");
        }

        var _problemService = _problemServiceFactory.GetServiceById(problemId.Value);
        var _problem = _problemService.GetProblem(problemId.Value);

        if (_problem == null || _problem.Status != ProblemStatus.Ready)
        {
            AddError("Problem not found!");
            return RedirectToAction("Index", "Course");
        }

        var file = Request.Form.Files[0];
        // read file and save it to file service
        var _solution = new S3Object
        {
            BucketName = _problem.FilesBucketName,
            FileName = file.FileName,
            ContentType = file.ContentType
        };

        _solution.Content = file.OpenReadStream();

        var submission = await _checkerService.CreateSubmissionAsync(_solution, _problem, SecurityContext.User!);

        _problemService.AddSolutionSubmission(_problem, submission);

        return Json(new { submissionId = submission.Id });
    }

    [HttpGet]
    public IActionResult Edit([FromQuery] int? problemId = default!)
    {
        if (problemId == null)
        {
            AddError("Problem not found!");
            return RedirectToAction("Index", "Course");
        }

        var _problemService = _problemServiceFactory.GetServiceById(problemId.Value);
        var _problem = _problemService.GetProblem(problemId.Value);

        if (_problem == null)
        {
            AddError("Problem not found!");
            return RedirectToAction("Index", "Course");
        }

        var _output = Mapper.Map<EditProblemViewModel>(_problem);

        // get files in bucket
        var _files = _fileService.GetFilesInBucket(_problem.FilesBucketName);
        _output.Files = _files.Select(x => x.BucketName + "/" + x.FileName).ToList();

        return View(_output);
    }

    [HttpPost]
    // upload endpoint for uploading file multipart/form-data
    public async Task<IActionResult> Upload([FromQuery] string bucketName)
    {
        var file = Request.Form.Files[0];
        // read file and save it to file service
        var _s3object = new S3Object
        {
            BucketName = bucketName,
            FileName = file.FileName,
            ContentType = file.ContentType
        };

        _s3object.Content = file.OpenReadStream();

        try
        {
            await _fileService.UploadFileAsync(_s3object);
        }
        catch (Exception e)
        {
            return BadRequest($"<span class=\"fw-600\">{file.FileName}</span> {e.Message}");
        }

        return Ok(bucketName + "/" + file.FileName);
    }

    [HttpDelete]
    public async Task<IActionResult> Remove()
    {
        // read raw body
        var bodyReader = await Request.BodyReader.ReadAsync();
        var fileLocation = EncodingExtensions.GetString(Encoding.UTF8, bodyReader.Buffer);

        var bucket = fileLocation.Split("/")[0];
        var fileName = fileLocation.Split("/")[1];

        await _fileService.DeleteFileAsync(bucket, fileName);

        return Ok();
    }

    // route is /problem/load/{bucket}/{fileName}
    [HttpGet]
    [Route("problem/load/{fileLocation}")]
    public async Task<IActionResult> Load(string fileLocation)
    {
        fileLocation = HttpUtility.UrlDecode(fileLocation);
        var bucket = fileLocation.Split("/")[0];
        var fileName = fileLocation.Split("/")[1];

        var _s3object = await _fileService.DownloadFileAsync(bucket, fileName);

        if (_s3object == null)
        {
            return NotFound();
        }

        Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");

        return File(_s3object.Content, _s3object.ContentType);
    }

    [HttpGet]
    public IActionResult Check([FromQuery] int? problemId)
    {
        if (problemId == null)
        {
            AddError("Problem not found!");
            return RedirectToAction("Index", "Course");
        }

        var _problemService = _problemServiceFactory.GetServiceById(problemId.Value);
        var _problem = _problemService.GetProblem(problemId.Value);

        if (_problem == null)
        {
            AddError("Problem not found!");
            return RedirectToAction("Index", "Course");
        }

        // get files in bucket
        var _files = _fileService.GetFilesInBucket(_problem.FilesBucketName);

        var _output = new CheckStructureViewModel();

        // README.md
        _output.ReadmePresent = _files.Any(x => x.FileName == "README.md");
        _output.ReadmeError = !_output.ReadmePresent ? "file is missing" : null;

        // bad source
        _output.BadSourcePresent = _files.Any(x => x.FileName == "main.cpp");
        _output.BadSourceError = !_output.BadSourcePresent ? "file is missing" : null;

        // good source
        _output.GoodSourcePresent = _files.Any(x => x.FileName == "solution_main.cpp");
        _output.GoodSourceError = !_output.GoodSourcePresent ? "file is missing" : null;

        // test cases
        _output.InputTestsCount = _files.Count(x => x.FileName.StartsWith("test") && x.FileName.EndsWith(".in"));
        _output.InputTestsError = _output.InputTestsCount == 0 ? "no input tests found" : null;
        _output.RefTestsCount = _files.Count(x => x.FileName.StartsWith("test") && x.FileName.EndsWith(".ref"));
        _output.RefTestsError = _output.RefTestsCount == 0 ? "no output tests found" : null;

        _output.SolutionOk = _problem.SolutionSubmission?.Status == SubmissionStatus.Passed;

        // if solution is ok, update problem status
        if (_output.SolutionOk)
        {
            _problemService.MakeProblemReady(_problem);
        }

        return PartialView("_CheckStructurePartial", _output);
    }

    [HttpGet]
    public IActionResult DownloadSource([FromQuery] int? problemId)
    {
        if (problemId == null)
        {
            AddError("Problem not found!");
            return RedirectToAction("Index", "Course");
        }

        var _problemService = _problemServiceFactory.GetServiceById(problemId.Value);
        var _problem = _problemService.GetProblem(problemId.Value);

        if (_problem == null || _problem.Status != ProblemStatus.Ready)
        {
            AddError("Problem not found!");
            return RedirectToAction("Index", "Course");
        }

        // get source
        var _source = _problemService.GetProblemSource(_problem);

        var _memorySource = new MemoryStream();
        _source.Content.CopyTo(_memorySource);


        return File(_memorySource.ToArray(), "text/plain", $"{_problem.Name}.cpp");
    }

    [HttpGet]
    public async Task<IActionResult> CheckSolution([FromQuery] int? problemId)
    {
        if (problemId == null)
        {
            AddError("Problem not found!");
            return RedirectToAction("Index", "Course");
        }

        var _problemService = _problemServiceFactory.GetServiceById(problemId.Value);
        var _problem = _problemService.GetProblem(problemId.Value);

        if (_problem == null)
        {
            AddError("Problem not found!");
            return RedirectToAction("Index", "Course");
        }

        // download solution
        var _solution = _problemService.GetProblemSolution(_problem);

        if (_solution == null)
        {
            AddError("Solution not found!");
            return RedirectToAction("Index", "Course");
        }

        var submission = await _checkerService.CreateSubmissionAsync(_solution, _problem, SecurityContext.User!);

        _problemService.AddSolutionSubmission(_problem, submission);

        return Json(new { submissionId = submission.Id });
    }

    [HttpGet]
    public async Task<IActionResult> CheckSubmission([FromQuery] string? submissionId)
    {
        if (submissionId == null)
        {
            AddError("Submission not found!");
            return RedirectToAction("Index", "Course");
        }

        var submission = await _checkerService.GetSubmissionAsync(submissionId);

        if (submission == null)
        {
            AddError("Submission not found!");
            return RedirectToAction("Index", "Course");
        }

        if (submission.Status == SubmissionStatus.Failed)
        {
            var _problemService = _problemServiceFactory.GetServiceById(submission.Problem.Id);
            var errors = _problemService.GetSubmissionErrors(submission);

            return Json(new { status = submission.Status.ToString(), errors });
        }

        return Json(new { status = submission.Status.ToString() });
    }
}
