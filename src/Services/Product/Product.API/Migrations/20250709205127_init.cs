using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Product.API.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NamePersian = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DescriptionPersian = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NamePersian = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DescriptionPersian = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BrandPersian = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscountPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiscountEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TagsPersian = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    Dimensions = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ColorPersian = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    OrderCount = table.Column<int>(type: "int", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    IsOnSale = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NamePersian = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ValuePersian = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributes_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AltTextPersian = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "DescriptionPersian", "ImageUrl", "IsActive", "Name", "NamePersian", "ParentCategoryId", "SortOrder", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("77441269-c867-41e1-9a7a-e648768ecc09"), new DateTime(2025, 7, 9, 20, 51, 27, 476, DateTimeKind.Utc).AddTicks(7172), "System", "Books and educational materials", "کتاب و مواد آموزشی", "", true, "Books", "کتاب", null, 3, new DateTime(2025, 7, 9, 20, 51, 27, 476, DateTimeKind.Utc).AddTicks(7173), "" },
                    { new Guid("951ccf06-0e4d-401c-b73d-816632f3e415"), new DateTime(2025, 7, 9, 20, 51, 27, 476, DateTimeKind.Utc).AddTicks(7126), "System", "Clothing and fashion items", "لباس و اقلام مد", "", true, "Clothing", "پوشاک", null, 2, new DateTime(2025, 7, 9, 20, 51, 27, 476, DateTimeKind.Utc).AddTicks(7126), "" },
                    { new Guid("b3f422f5-be2e-440c-be34-eccd5c325d8c"), new DateTime(2025, 7, 9, 20, 51, 27, 476, DateTimeKind.Utc).AddTicks(7075), "System", "Electronic devices and accessories", "دستگاه‌ها و لوازم جانبی الکترونیکی", "", true, "Electronics", "لوازم الکترونیکی", null, 1, new DateTime(2025, 7, 9, 20, 51, 27, 476, DateTimeKind.Utc).AddTicks(7077), "" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "BrandPersian", "CategoryId", "Color", "ColorPersian", "CreatedAt", "CreatedBy", "Description", "DescriptionPersian", "Dimensions", "DiscountEndDate", "DiscountPrice", "DiscountStartDate", "ImageUrl", "IsActive", "IsFeatured", "IsOnSale", "Name", "NamePersian", "OrderCount", "Price", "Size", "Stock", "Tags", "TagsPersian", "UpdatedAt", "UpdatedBy", "ViewCount", "Weight" },
                values: new object[,]
                {
                    { new Guid("e6381bed-5470-4511-98e5-d24b6d3c5348"), "FashionBrand", "فشن برند", new Guid("951ccf06-0e4d-401c-b73d-816632f3e415"), "", "", new DateTime(2025, 7, 9, 20, 51, 27, 476, DateTimeKind.Utc).AddTicks(7308), "System", "Comfortable cotton t-shirt", "تی شرت پنبه‌ای راحت", "", null, null, null, "", true, false, false, "T-Shirt", "تی شرت", 0, 19.99m, "", 200, "tshirt, cotton, clothing", "تی شرت، پنبه، لباس", new DateTime(2025, 7, 9, 20, 51, 27, 476, DateTimeKind.Utc).AddTicks(7309), "", 0, 0.0 },
                    { new Guid("f41639f0-51f6-4879-9e71-898b644d133c"), "TechBrand", "تک برند", new Guid("b3f422f5-be2e-440c-be34-eccd5c325d8c"), "", "", new DateTime(2025, 7, 9, 20, 51, 27, 476, DateTimeKind.Utc).AddTicks(7296), "System", "Latest smartphone with advanced features", "آخرین گوشی هوشمند با ویژگی‌های پیشرفته", "", null, null, null, "", true, true, true, "Smartphone", "گوشی هوشمند", 0, 599.99m, "", 100, "smartphone, mobile, tech", "گوشی هوشمند، موبایل، تکنولوژی", new DateTime(2025, 7, 9, 20, 51, 27, 476, DateTimeKind.Utc).AddTicks(7297), "", 0, 0.0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributes_ProductId",
                table: "ProductAttributes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAttributes");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
