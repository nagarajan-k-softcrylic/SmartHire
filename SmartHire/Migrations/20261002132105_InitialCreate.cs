using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHire.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndexingJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DocumentsDiscovered = table.Column<int>(type: "int", nullable: false),
                    DocumentsIndexed = table.Column<int>(type: "int", nullable: false),
                    DocumentsFailed = table.Column<int>(type: "int", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorSummary = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexingJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResumeDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlobName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BlobUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContainerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    ContentHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlobLastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LastIndexedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CandidateProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResumeDocumentId = table.Column<int>(type: "int", nullable: false),
                    CandidateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalExperienceYears = table.Column<double>(type: "float", nullable: true),
                    Skills = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResumeSummary = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateProfiles_ResumeDocuments_ResumeDocumentId",
                        column: x => x.ResumeDocumentId,
                        principalTable: "ResumeDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeChunks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResumeDocumentId = table.Column<int>(type: "int", nullable: false),
                    ChunkIndex = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeChunks_ResumeDocuments_ResumeDocumentId",
                        column: x => x.ResumeDocumentId,
                        principalTable: "ResumeDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScreeningRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestedByUserId = table.Column<int>(type: "int", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredSkills = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimumExperienceYears = table.Column<int>(type: "int", nullable: false),
                    PreferredExperienceYears = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreeningRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreeningRequests_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GrantedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserApplications_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserApplications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeEmbeddings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResumeChunkId = table.Column<int>(type: "int", nullable: false),
                    SearchIndexDocumentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmbeddingModel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VectorDimensions = table.Column<int>(type: "int", nullable: false),
                    GeneratedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeEmbeddings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeEmbeddings_ResumeChunks_ResumeChunkId",
                        column: x => x.ResumeChunkId,
                        principalTable: "ResumeChunks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScreeningResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScreeningRequestId = table.Column<int>(type: "int", nullable: false),
                    CandidateProfileId = table.Column<int>(type: "int", nullable: false),
                    MatchPercentage = table.Column<double>(type: "float", nullable: false),
                    SkillMatchScore = table.Column<double>(type: "float", nullable: false),
                    ExperienceMatchScore = table.Column<double>(type: "float", nullable: false),
                    Strengths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MissingSkills = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiRecommendation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreeningResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreeningResults_CandidateProfiles_CandidateProfileId",
                        column: x => x.CandidateProfileId,
                        principalTable: "CandidateProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScreeningResults_ScreeningRequests_ScreeningRequestId",
                        column: x => x.ScreeningRequestId,
                        principalTable: "ScreeningRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Applications",
                columns: new[] { "Id", "Code", "IsActive", "Name" },
                values: new object[] { 1, "RESUME_AI", true, "SmartHire" });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Code",
                table: "Applications",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_ResumeDocumentId",
                table: "CandidateProfiles",
                column: "ResumeDocumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResumeChunks_ResumeDocumentId",
                table: "ResumeChunks",
                column: "ResumeDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeDocuments_BlobName",
                table: "ResumeDocuments",
                column: "BlobName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResumeEmbeddings_ResumeChunkId",
                table: "ResumeEmbeddings",
                column: "ResumeChunkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScreeningRequests_RequestedByUserId",
                table: "ScreeningRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreeningResults_CandidateProfileId",
                table: "ScreeningResults",
                column: "CandidateProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreeningResults_ScreeningRequestId",
                table: "ScreeningResults",
                column: "ScreeningRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_ApplicationId",
                table: "UserApplications",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_UserId_ApplicationId",
                table: "UserApplications",
                columns: new[] { "UserId", "ApplicationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SubjectId",
                table: "Users",
                column: "SubjectId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "IndexingJobs");

            migrationBuilder.DropTable(
                name: "ResumeEmbeddings");

            migrationBuilder.DropTable(
                name: "ScreeningResults");

            migrationBuilder.DropTable(
                name: "UserApplications");

            migrationBuilder.DropTable(
                name: "ResumeChunks");

            migrationBuilder.DropTable(
                name: "CandidateProfiles");

            migrationBuilder.DropTable(
                name: "ScreeningRequests");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "ResumeDocuments");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
