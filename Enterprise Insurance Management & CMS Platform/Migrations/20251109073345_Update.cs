using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enterprise_Insurance_Management___CMS_Platform.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPolicies_AspNetUsers_CustomerId",
                table: "CustomerPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPolicies_Policies_PolicyId",
                table: "CustomerPolicies");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPolicies_CustomerId",
                table: "CustomerPolicies");

            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "CustomerPolicies");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPolicies_CustomerId_PolicyId",
                table: "CustomerPolicies",
                columns: new[] { "CustomerId", "PolicyId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPolicies_AspNetUsers_CustomerId",
                table: "CustomerPolicies",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPolicies_Policies_PolicyId",
                table: "CustomerPolicies",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPolicies_AspNetUsers_CustomerId",
                table: "CustomerPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPolicies_Policies_PolicyId",
                table: "CustomerPolicies");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPolicies_CustomerId_PolicyId",
                table: "CustomerPolicies");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "CustomerPolicies",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPolicies_CustomerId",
                table: "CustomerPolicies",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPolicies_AspNetUsers_CustomerId",
                table: "CustomerPolicies",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPolicies_Policies_PolicyId",
                table: "CustomerPolicies",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
