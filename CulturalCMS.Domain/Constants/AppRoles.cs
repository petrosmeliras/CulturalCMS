using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Domain.Constants
{
    public static class AppRoles
    {
        public const string Contributor = "Contributor";
        public const string Curator = "Curator";
        public const string Admin = "Admin";

        public const string CuratorOrAdmin = Curator + "," + Admin;
        public const string ContributorOrAdmin = Contributor + "," + Admin;
        public const string AnyRole = Contributor + "," + Curator + "," + Admin;
    }
}
