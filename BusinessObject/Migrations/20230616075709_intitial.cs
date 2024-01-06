using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    public partial class intitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "majors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "(UUID())"),
                    short_name = table.Column<string>(type: "longtext", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_majors", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "(UUID())"),
                    name = table.Column<string>(type: "longtext", nullable: false),
                    content = table.Column<string>(type: "longtext", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    image = table.Column<string>(type: "longtext", nullable: true),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "(UUID())"),
                    fullname = table.Column<string>(type: "longtext", nullable: false),
                    password = table.Column<string>(type: "longtext", nullable: true),
                    email = table.Column<string>(type: "longtext", nullable: false),
                    phone = table.Column<string>(type: "longtext", nullable: true),
                    birthday = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    gender = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: false),
                    skill = table.Column<string>(type: "longtext", nullable: false),
                    token = table.Column<string>(type: "longtext", nullable: true),
                    other_contact = table.Column<string>(type: "longtext", nullable: true),
                    avatar = table.Column<string>(type: "longtext", nullable: true),
                    theme = table.Column<string>(type: "longtext", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    is_admin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURRENT_DATE())"),
                    verify_code = table.Column<string>(type: "longtext", nullable: true),
                    end_date_premium = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    last_login_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "(UUID())"),
                    transaction_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    transaction_code = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_transactions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_majors",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    major_id = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_majors", x => new { x.user_id, x.major_id });
                    table.ForeignKey(
                        name: "FK_user_majors_majors_major_id",
                        column: x => x.major_id,
                        principalTable: "majors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_majors_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "application_majors",
                columns: table => new
                {
                    application_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    major_id = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_majors", x => new { x.application_id, x.major_id });
                    table.ForeignKey(
                        name: "FK_application_majors_majors_major_id",
                        column: x => x.major_id,
                        principalTable: "majors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "applications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "(UUID())"),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    confirmed_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    description = table.Column<string>(type: "longtext", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    group_id = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications", x => x.id);
                    table.ForeignKey(
                        name: "FK_applications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "assigned_tasks",
                columns: table => new
                {
                    task_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    assigned_for_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    assigned_by_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    assigned_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assigned_tasks", x => new { x.assigned_by_id, x.assigned_for_id, x.task_id });
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "(UUID())"),
                    content = table.Column<string>(type: "longtext", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    task_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    member_id = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "feedbacks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "(UUID())"),
                    content = table.Column<string>(type: "longtext", nullable: false),
                    rating = table.Column<float>(type: "float", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    feedbacked_by_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    feedbacked_for_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    group_id = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedbacks", x => x.id);
                    table.ForeignKey(
                        name: "FK_feedbacks_users_feedbacked_by_id",
                        column: x => x.feedbacked_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_feedbacks_users_feedbacked_for_id",
                        column: x => x.feedbacked_for_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "group_majors",
                columns: table => new
                {
                    group_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    major_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    member_count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_majors", x => new { x.group_id, x.major_id });
                    table.ForeignKey(
                        name: "FK_group_majors_majors_major_id",
                        column: x => x.major_id,
                        principalTable: "majors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "(UUID())"),
                    name = table.Column<string>(type: "longtext", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    group_size = table.Column<int>(type: "int", nullable: false),
                    member_count = table.Column<int>(type: "int", nullable: false),
                    school_name = table.Column<string>(type: "longtext", nullable: true),
                    class_name = table.Column<string>(type: "longtext", nullable: true),
                    subject_name = table.Column<string>(type: "longtext", nullable: true),
                    description = table.Column<string>(type: "longtext", nullable: true),
                    skill = table.Column<string>(type: "longtext", nullable: true),
                    avatar = table.Column<string>(type: "longtext", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    current_milestone_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    created_by_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    theme = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "(UUID())"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    group_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    joined_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    left_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_members_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "milestones",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "(UUID())"),
                    name = table.Column<string>(type: "longtext", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: false),
                    order = table.Column<int>(type: "int", nullable: false),
                    group_id = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_milestones", x => x.id);
                    table.ForeignKey(
                        name: "FK_milestones_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "(UUID())"),
                    name = table.Column<string>(type: "longtext", nullable: false),
                    start_date_deadline = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_date_deadline = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    finished_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    important_level = table.Column<int>(type: "int", nullable: false),
                    estimated_days = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    group_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_by_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    main_task_id = table.Column<Guid>(type: "char(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.id);
                    table.ForeignKey(
                        name: "FK_tasks_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tasks_members_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tasks_tasks_main_task_id",
                        column: x => x.main_task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_application_majors_major_id",
                table: "application_majors",
                column: "major_id");

            migrationBuilder.CreateIndex(
                name: "IX_applications_group_id",
                table: "applications",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_applications_user_id",
                table: "applications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_assigned_tasks_assigned_for_id",
                table: "assigned_tasks",
                column: "assigned_for_id");

            migrationBuilder.CreateIndex(
                name: "IX_assigned_tasks_task_id",
                table: "assigned_tasks",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_task_id",
                table: "comments",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_feedbacked_by_id",
                table: "feedbacks",
                column: "feedbacked_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_feedbacked_for_id",
                table: "feedbacks",
                column: "feedbacked_for_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_group_id",
                table: "feedbacks",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_group_majors_major_id",
                table: "group_majors",
                column: "major_id");

            migrationBuilder.CreateIndex(
                name: "IX_groups_created_by_id",
                table: "groups",
                column: "created_by_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_groups_current_milestone_id",
                table: "groups",
                column: "current_milestone_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_members_group_id",
                table: "members",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_members_user_id",
                table: "members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_milestones_group_id",
                table: "milestones",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_created_by_id",
                table: "tasks",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_group_id",
                table: "tasks",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_main_task_id",
                table: "tasks",
                column: "main_task_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id",
                table: "transactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_majors_major_id",
                table: "user_majors",
                column: "major_id");

            migrationBuilder.AddForeignKey(
                name: "FK_application_majors_applications_application_id",
                table: "application_majors",
                column: "application_id",
                principalTable: "applications",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_applications_groups_group_id",
                table: "applications",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_assigned_tasks_members_assigned_by_id",
                table: "assigned_tasks",
                column: "assigned_by_id",
                principalTable: "members",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_assigned_tasks_members_assigned_for_id",
                table: "assigned_tasks",
                column: "assigned_for_id",
                principalTable: "members",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_assigned_tasks_tasks_task_id",
                table: "assigned_tasks",
                column: "task_id",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_tasks_task_id",
                table: "comments",
                column: "task_id",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_groups_group_id",
                table: "feedbacks",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_group_majors_groups_group_id",
                table: "group_majors",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_groups_members_created_by_id",
                table: "groups",
                column: "created_by_id",
                principalTable: "members",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_groups_milestones_current_milestone_id",
                table: "groups",
                column: "current_milestone_id",
                principalTable: "milestones",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_members_groups_group_id",
                table: "members");

            migrationBuilder.DropForeignKey(
                name: "FK_milestones_groups_group_id",
                table: "milestones");

            migrationBuilder.DropTable(
                name: "application_majors");

            migrationBuilder.DropTable(
                name: "assigned_tasks");

            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "feedbacks");

            migrationBuilder.DropTable(
                name: "group_majors");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "user_majors");

            migrationBuilder.DropTable(
                name: "applications");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "majors");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "milestones");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
