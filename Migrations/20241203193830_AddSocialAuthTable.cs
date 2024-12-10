using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace certificated_unemi.Migrations
{
    /// <inheritdoc />
    public partial class AddSocialAuthTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthenticationDate",
                table: "SocialAuths");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "SocialAuths",
                newName: "RefreshToken");

            migrationBuilder.RenameColumn(
                name: "LinkedInToken",
                table: "SocialAuths",
                newName: "ProviderId");

            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "SocialAuths",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "SocialAuths",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "SocialAuths");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "SocialAuths");

            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "SocialAuths",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "SocialAuths",
                newName: "LinkedInToken");

            migrationBuilder.AddColumn<DateTime>(
                name: "AuthenticationDate",
                table: "SocialAuths",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
