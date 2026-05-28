using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CascadeUserDeletes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_booking_user_visitor_id",
                table: "booking");

            migrationBuilder.DropForeignKey(
                name: "fk_review_user_reviewer_id",
                table: "review");

            migrationBuilder.AddForeignKey(
                name: "fk_booking_user_visitor_id",
                table: "booking",
                column: "visitor_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_review_user_reviewer_id",
                table: "review",
                column: "reviewer_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_booking_user_visitor_id",
                table: "booking");

            migrationBuilder.DropForeignKey(
                name: "fk_review_user_reviewer_id",
                table: "review");

            migrationBuilder.AddForeignKey(
                name: "fk_booking_user_visitor_id",
                table: "booking",
                column: "visitor_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_review_user_reviewer_id",
                table: "review",
                column: "reviewer_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
