using Microsoft.EntityFrameworkCore.Migrations;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Extensions;

#nullable disable

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateViews : Migration
    {
        private readonly string[] _viewNames =
        [
            "view_countries",
            "view_genders",
            "view_roles",
            "view_statuses",
            "view_users"
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var viewName in _viewNames)
                migrationBuilder.CreateView(viewName);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var viewName in _viewNames.Reverse())
                migrationBuilder.DropView(viewName);
        }
    }
}
