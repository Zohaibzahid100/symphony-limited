using Microsoft.AspNetCore.Http;

namespace WebApplication1.Helpers
{
    // Small helper so we can keep login / registration state in Session
    // without wiring up full ASP.NET Identity for this project.
    public static class SessionKeys
    {
        public const string UserId = "UserId";
        public const string StudentId = "StudentId";
        public const string FullName = "FullName";
        public const string Role = "Role";
        public const string AppliedExamIds = "AppliedExamIds"; // comma separated EntranceExamIds applied for in this session
    }

    public static class SessionExtensions
    {
        public static bool IsLoggedIn(this ISession session)
        {
            return session.GetInt32(SessionKeys.UserId).HasValue;
        }

        public static int? GetStudentId(this ISession session)
        {
            return session.GetInt32(SessionKeys.StudentId);
        }

        public static List<int> GetAppliedExamIds(this ISession session)
        {
            var raw = session.GetString(SessionKeys.AppliedExamIds);
            if (string.IsNullOrEmpty(raw)) return new List<int>();
            return raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(int.Parse)
                       .ToList();
        }

        public static void AddAppliedExamId(this ISession session, int examId)
        {
            var ids = session.GetAppliedExamIds();
            if (!ids.Contains(examId))
            {
                ids.Add(examId);
                session.SetString(SessionKeys.AppliedExamIds, string.Join(",", ids));
            }
        }
    }
}
