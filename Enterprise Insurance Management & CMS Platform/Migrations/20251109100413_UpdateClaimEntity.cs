using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enterprise_Insurance_Management___CMS_Platform.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClaimEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClaimNumber",
                table: "Claims",
                newName: "ClaimReason");

            migrationBuilder.AddColumn<Guid>(
                name: "ClaimEntityId",
                table: "Documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ClaimEntityId",
                table: "Documents",
                column: "ClaimEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_CustomerId",
                table: "Claims",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_PolicyId",
                table: "Claims",
                column: "PolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_AspNetUsers_CustomerId",
                table: "Claims",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Policies_PolicyId",
                table: "Claims",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Claims_ClaimEntityId",
                table: "Documents",
                column: "ClaimEntityId",
                principalTable: "Claims",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_AspNetUsers_CustomerId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Policies_PolicyId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Claims_ClaimEntityId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ClaimEntityId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Claims_CustomerId",
                table: "Claims");

            migrationBuilder.DropIndex(
                name: "IX_Claims_PolicyId",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ClaimEntityId",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "ClaimReason",
                table: "Claims",
                newName: "ClaimNumber");
        }
    }
}
