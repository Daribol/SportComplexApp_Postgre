using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportComplexApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTriggers : Migration
    {
        private readonly string[] _tables = new[]
        {
            "Facilities", "Sports", "AspNetUsers", "Reservations",
            "SpaReservations", "SpaServices", "Tournaments",
            "TournamentRegistrations", "Trainers", "TrainerSessions", "SportTrainers"
        };
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var table in _tables)
            {
                migrationBuilder.Sql($@"
                    CREATE TRIGGER [22180008].trg_{table}_Audit
                    ON [22180008].[{table}]
                    AFTER INSERT, UPDATE
                    AS
                    BEGIN
                        DECLARE @Operation VARCHAR(10);
                        
                        -- Проверка дали операцията е UPDATE или INSERT
                        IF EXISTS(SELECT 1 FROM deleted) 
                            SET @Operation = 'UPDATE';
                        ELSE 
                            SET @Operation = 'INSERT';

                        -- Записваме в журнала
                        IF EXISTS(SELECT 1 FROM inserted)
                        BEGIN
                            INSERT INTO [22180008].[Logs_22180008] (TableName, OperationType, OperationDate)
                            VALUES ('{table}', @Operation, GETDATE());
                        END
                    END
                ");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var table in _tables)
            {
                migrationBuilder.Sql($"DROP TRIGGER IF EXISTS [22180008].trg_{table}_Audit;");
            }
        }
    }
}
