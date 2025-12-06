using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VibeApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Admin role if not exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""Name"" = 'Admin') THEN
                        INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                        VALUES (gen_random_uuid()::text, 'Admin', 'ADMIN', gen_random_uuid()::text);
                    END IF;
                END $$;
            ");

            // Assign Admin role to user with email rnd.develop@gmail.com if user exists
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    v_user_id text;
                    v_role_id text;
                BEGIN
                    -- Find user by email
                    SELECT ""Id"" INTO v_user_id
                    FROM ""AspNetUsers""
                    WHERE ""Email"" = 'rnd.develop@gmail.com'
                    LIMIT 1;

                    -- If user exists, assign Admin role
                    IF v_user_id IS NOT NULL THEN
                        -- Get Admin role ID
                        SELECT ""Id"" INTO v_role_id
                        FROM ""AspNetRoles""
                        WHERE ""Name"" = 'Admin'
                        LIMIT 1;

                        -- Assign role if not already assigned
                        IF NOT EXISTS (
                            SELECT 1 FROM ""AspNetUserRoles""
                            WHERE ""UserId"" = v_user_id AND ""RoleId"" = v_role_id
                        ) THEN
                            INSERT INTO ""AspNetUserRoles"" (""UserId"", ""RoleId"")
                            VALUES (v_user_id, v_role_id);
                        END IF;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove Admin role assignment from user
            migrationBuilder.Sql(@"
                DELETE FROM ""AspNetUserRoles""
                WHERE ""RoleId"" IN (
                    SELECT ""Id"" FROM ""AspNetRoles"" WHERE ""Name"" = 'Admin'
                )
                AND ""UserId"" IN (
                    SELECT ""Id"" FROM ""AspNetUsers"" WHERE ""Email"" = 'rnd.develop@gmail.com'
                );
            ");

            // Remove Admin role
            migrationBuilder.Sql(@"
                DELETE FROM ""AspNetRoles"" WHERE ""Name"" = 'Admin';
            ");
        }
    }
}

