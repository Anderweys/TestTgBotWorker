using FluentMigrator;

namespace TestTgBot.Db.Migrations;

[Migration(202507311200)]
public class AddMessagesTable : Migration
{
    public override void Up()
    {
        Create.Table("Messages")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("ChatId").AsInt64().NotNullable()
            .WithColumn("MessageId").AsInt32().NotNullable()
            .WithColumn("DeleteAt").AsDateTime().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Messages");
    }
}