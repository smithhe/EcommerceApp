using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    LastName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.UniqueConstraint("AK_AspNetUsers_UserName", x => x.UserName);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Summary = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.CheckConstraint("CHK_CCreatedDate", "CreatedDate <= LastModifiedDate");
                    table.CheckConstraint("CHK_CLastModifiedDate", "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<double>(type: "double", nullable: false),
                    PayPalRequestId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.CheckConstraint("CHK_OCreatedDate", "CreatedDate <= LastModifiedDate");
                    table.CheckConstraint("CHK_OLastModifiedDate", "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate");
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    Price = table.Column<double>(type: "double", nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantityAvailable = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "longtext", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.CheckConstraint("CHK_PCreatedDate", "CreatedDate <= LastModifiedDate");
                    table.CheckConstraint("CHK_PLastModifiedDate", "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate");
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    ProductDescription = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    ProductSku = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "double", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.CheckConstraint("CHK_OICreatedDate", "CreatedDate <= LastModifiedDate");
                    table.CheckConstraint("CHK_OILastModifiedDate", "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate");
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderToken = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderKeys_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.CheckConstraint("CHK_CICreatedDate", "CreatedDate <= LastModifiedDate");
                    table.CheckConstraint("CHK_CILastModifiedDate", "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate");
                    table.ForeignKey(
                        name: "FK_CartItems_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    Stars = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "longtext", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.CheckConstraint("CHK_RCreatedDate", "CreatedDate <= LastModifiedDate");
                    table.CheckConstraint("CHK_RLastModifiedDate", "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate");
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_UserName",
                        column: x => x.UserName,
                        principalTable: "AspNetUsers",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name", "Summary" },
                values: new object[,]
                {
                    { 1, "Harold", new DateTime(2024, 4, 8, 23, 54, 50, 815, DateTimeKind.Local).AddTicks(5029), null, null, "Laptops", "Explore our range of laptops." },
                    { 2, "Harold", new DateTime(2024, 4, 8, 23, 54, 50, 815, DateTimeKind.Local).AddTicks(5085), null, null, "Phones", "Discover the latest smartphones." },
                    { 3, "Harold", new DateTime(2024, 4, 8, 23, 54, 50, 815, DateTimeKind.Local).AddTicks(5088), null, null, "Tablets", "Browse our collection of tablets." }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "AverageRating", "CategoryId", "CreatedBy", "CreatedDate", "Description", "ImageUrl", "LastModifiedBy", "LastModifiedDate", "Name", "Price", "QuantityAvailable" },
                values: new object[,]
                {
                    { 1, 0m, 1, "Harold", new DateTime(2024, 4, 8, 23, 54, 50, 815, DateTimeKind.Local).AddTicks(5166), "This is a killer laptop that can handle all your home needs", "https://smith-ecommerce-app.s3.amazonaws.com/laptop1.jpg", null, null, "Laptop 1", 299.99000000000001, 5 },
                    { 2, 0m, 1, "Harold", new DateTime(2024, 4, 8, 23, 54, 50, 815, DateTimeKind.Local).AddTicks(5171), "This is a killer laptop that can handle all your home needs", "https://smith-ecommerce-app.s3.amazonaws.com/laptop2.jpg", null, null, "Laptop 2", 499.99000000000001, 5 },
                    { 3, 0m, 1, "Harold", new DateTime(2024, 4, 8, 23, 54, 50, 815, DateTimeKind.Local).AddTicks(5175), "This is a killer laptop that can handle all your home needs", "https://smith-ecommerce-app.s3.amazonaws.com/laptop3.jpg", null, null, "Laptop 3", 999.99000000000001, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderKeys_OrderId",
                table: "OrderKeys",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserName",
                table: "Reviews",
                column: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "OrderKeys");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
