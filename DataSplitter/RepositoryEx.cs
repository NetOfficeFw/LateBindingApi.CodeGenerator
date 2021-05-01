using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

namespace DataSplitter
{
    public static class RepositoryEx
    {
        public static bool HasChanges(this IRepository repo)
        {
            var status = repo.RetrieveStatus();

            return status.Staged.Any() || status.Added.Any() || status.Removed.Any() || status.RenamedInIndex.Any();
        }
    }
}
