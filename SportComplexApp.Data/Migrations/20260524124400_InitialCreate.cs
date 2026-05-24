using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportComplexApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "22180008");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastModified_22180008 = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModified_22180008 = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs_22180008",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs_22180008", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpaServices",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ProcedureDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false, defaultValue: 60),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModified_22180008 = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "22180008",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "22180008",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "22180008",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "22180008",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "22180008",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "22180008",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "22180008",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "22180008",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "22180008",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trainers",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified_22180008 = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trainers_AspNetUsers_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "22180008",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sports",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FacilityId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    MinPeople = table.Column<int>(type: "int", nullable: false),
                    MaxPeople = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModified_22180008 = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sports_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalSchema: "22180008",
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpaReservations",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SpaServiceId = table.Column<int>(type: "int", nullable: false),
                    ReservationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfPeople = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    LastModified_22180008 = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpaReservations_AspNetUsers_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "22180008",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpaReservations_SpaServices_SpaServiceId",
                        column: x => x.SpaServiceId,
                        principalSchema: "22180008",
                        principalTable: "SpaServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainerSessions",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainerId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified_22180008 = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainerSessions_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalSchema: "22180008",
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false),
                    TrainerId = table.Column<int>(type: "int", nullable: true),
                    ReservationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    NumberOfPeople = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    LastModified_22180008 = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_AspNetUsers_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "22180008",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Sports_SportId",
                        column: x => x.SportId,
                        principalSchema: "22180008",
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalSchema: "22180008",
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SportTrainers",
                schema: "22180008",
                columns: table => new
                {
                    SportId = table.Column<int>(type: "int", nullable: false),
                    TrainerId = table.Column<int>(type: "int", nullable: false),
                    LastModified_22180008 = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportTrainers", x => new { x.SportId, x.TrainerId });
                    table.ForeignKey(
                        name: "FK_SportTrainers_Sports_SportId",
                        column: x => x.SportId,
                        principalSchema: "22180008",
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SportTrainers_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalSchema: "22180008",
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModified_22180008 = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tournaments_Sports_SportId",
                        column: x => x.SportId,
                        principalSchema: "22180008",
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentRegistrations",
                schema: "22180008",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TournamentId = table.Column<int>(type: "int", nullable: false),
                    LastModified_22180008 = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentRegistrations_AspNetUsers_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "22180008",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TournamentRegistrations_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalSchema: "22180008",
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "22180008",
                table: "Facilities",
                columns: new[] { "Id", "ImageUrl", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSASkhPWQ1zqXLUUBYFlMhZn4yMaeMu8yVjZg&s", false, "Indoor Arena" },
                    { 2, "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSwhAHcH2_eq_owatwVzZDK2JyB4QcdFPya9g&s", false, "Tennis Center" },
                    { 3, "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQpZB-L2ZhjG6LXt6ByYv4vOYWu3lGvTvKNOQ&s", false, "Fitness Studio" }
                });

            migrationBuilder.InsertData(
                schema: "22180008",
                table: "SpaServices",
                columns: new[] { "Id", "Description", "Duration", "ImageUrl", "IsDeleted", "Name", "Price", "ProcedureDetails" },
                values: new object[,]
                {
                    { 1, "Relaxing full-body massage with essential oils.", 60, "https://images.unsplash.com/photo-1620050382792-434b5828873d?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8NXx8Q2xhc3NpYyUyMEZ1bGwlMjBCb2R5JTIwTWFzc2FnZXxlbnwwfDB8MHx8fDI%3D", false, "Classic Full-Body Massage", 55.00m, "Mild to medium pressure, lavender-almond oil." },
                    { 2, "Warm volcanic stones to soothe deep muscle tension.", 70, "https://images.unsplash.com/photo-1610402601271-5b4bd5b3eba4?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8SG90JTIwU3RvbmUlMjBUaGVyYXB5fGVufDB8MHwwfHx8Mg%3D%3D", false, "Hot Stone Therapy", 75.00m, "Progressive heating and placement along energy meridians." },
                    { 3, "Combined access to sauna and steam room.", 60, "https://images.unsplash.com/photo-1712659604528-b179a3634560?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8N3x8U2F1bmF8ZW58MHwwfDB8fHwy", false, "Sauna + Steam Room Package", 25.00m, "30 min sauna + 20 min steam, plus hydration." }
                });

            migrationBuilder.InsertData(
                schema: "22180008",
                table: "Trainers",
                columns: new[] { "Id", "Bio", "ClientId", "ImageUrl", "IsDeleted", "LastName", "Name" },
                values: new object[,]
                {
                    { 1, "Certified basketball coach with 8+ years of experience.", null, "/images/kiril_raikov.jpg", false, "Raikov", "Kiril" },
                    { 2, "Yoga & Pilates instructor focused on mobility and mindfulness.", null, "https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=500&h=500&fit=crop", false, "Markovska", "Vili" },
                    { 3, "CrossFit coach, competition-level conditioning specialist.", null, "/images/koceto.jpg", false, "Lefterov", "Kostadin" },
                    { 4, "Tennis training: technique, tactics, and matchplay.", null, "/images/grisho.jpg", false, "Dimitrov", "Grigor" }
                });

            migrationBuilder.InsertData(
                schema: "22180008",
                table: "Sports",
                columns: new[] { "Id", "Duration", "FacilityId", "ImageUrl", "IsDeleted", "MaxPeople", "MinPeople", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 60, 1, "https://images.unsplash.com/photo-1519766304817-4f37bda74a26?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MTF8fGJhc2tldGJhbGx8ZW58MHwwfDB8fHwy", false, 10, 2, "Basketball", 30.00m },
                    { 2, 60, 2, "https://images.unsplash.com/flagged/photo-1576972405668-2d020a01cbfa?q=80&w=1174&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D", false, 4, 2, "Tennis", 25.00m },
                    { 3, 60, 3, "https://images.unsplash.com/photo-1588286840104-8957b019727f?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8OXx8eW9nYXxlbnwwfDB8MHx8fDI%3D", false, 20, 4, "Yoga (group session)", 10.00m },
                    { 4, 50, 3, "https://images.unsplash.com/photo-1547226238-e53e98a8e59d?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MTc3fHxjcm9zc0ZpdHxlbnwwfDB8MHx8fDI%3D", false, 16, 4, "CrossFit (group session)", 15.00m },
                    { 5, 60, 1, "https://images.unsplash.com/photo-1659303388053-6078a001ea21?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MzV8fHRhYmxlJTIwdGVubmlzfGVufDB8MHwwfHx8Mg%3D%3D", false, 4, 2, "Table tennis", 10.00m }
                });

            migrationBuilder.InsertData(
                schema: "22180008",
                table: "SportTrainers",
                columns: new[] { "SportId", "TrainerId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 4 },
                    { 3, 2 },
                    { 3, 3 },
                    { 4, 3 }
                });

            migrationBuilder.InsertData(
                schema: "22180008",
                table: "Tournaments",
                columns: new[] { "Id", "Description", "EndDate", "ImageUrl", "IsDeleted", "Name", "SportId", "StartDate" },
                values: new object[,]
                {
                    { 1, "Amateur tennis tournament with group stages and knockouts.", new DateTime(2025, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "City Cup – Tennis", 2, new DateTime(2025, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Fast-paced 3-on-3 format, open for mixed teams.", new DateTime(2025, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Basket 3-on-3 Open", 1, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Sprint races across age groups.", new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Swim Sprint Challenge", 3, new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "The biggest upcoming tennis event of the year. Register now!", new DateTime(2026, 12, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Winter Grand Slam 2026", 2, new DateTime(2026, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "22180008",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "22180008",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "22180008",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "22180008",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "22180008",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "22180008",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "22180008",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ClientId",
                schema: "22180008",
                table: "Reservations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_SportId",
                schema: "22180008",
                table: "Reservations",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TrainerId",
                schema: "22180008",
                table: "Reservations",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_SpaReservations_ClientId",
                schema: "22180008",
                table: "SpaReservations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_SpaReservations_SpaServiceId",
                schema: "22180008",
                table: "SpaReservations",
                column: "SpaServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Sports_FacilityId",
                schema: "22180008",
                table: "Sports",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_SportTrainers_TrainerId",
                schema: "22180008",
                table: "SportTrainers",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRegistrations_ClientId",
                schema: "22180008",
                table: "TournamentRegistrations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRegistrations_TournamentId",
                schema: "22180008",
                table: "TournamentRegistrations",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_SportId",
                schema: "22180008",
                table: "Tournaments",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_ClientId",
                schema: "22180008",
                table: "Trainers",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerSessions_TrainerId",
                schema: "22180008",
                table: "TrainerSessions",
                column: "TrainerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "Logs_22180008",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "Reservations",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "SpaReservations",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "SportTrainers",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "TournamentRegistrations",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "TrainerSessions",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "SpaServices",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "Tournaments",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "Trainers",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "Sports",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "22180008");

            migrationBuilder.DropTable(
                name: "Facilities",
                schema: "22180008");
        }
    }
}
