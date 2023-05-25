namespace Data.Models.Enums
{
    public enum ProblemStatus
    {
        /// <summary>
        /// The problem is ready to be solved
        /// </summary>
        Ready = 0,

        /// <summary>
        /// The problem is incomplete (missing files or config)
        /// </summary>
        Incomplete = 1,

        /// <summary>
        /// The problem is hidden from the public
        /// </summary>
        Hidden = 2,
    }
}