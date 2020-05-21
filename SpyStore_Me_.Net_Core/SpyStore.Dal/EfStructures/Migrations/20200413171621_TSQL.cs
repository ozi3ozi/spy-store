using Microsoft.EntityFrameworkCore.Migrations;
using SpyStore.Dal.EfStructures.MigrationHelpers;

//An empty migration is what should be used to create SQL Server objects with T-SQL 
//(functions, stored precedures and views,...) since mixing generated code and custom 
//code can lead to issues when migrations need to be removed and recreated.
namespace SpyStore.Dal.EfStructures.Migrations
{
    public partial class TSQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            FunctionsHelper.CreateOrderTotalFunction(migrationBuilder);
            SprocsHelper.CreatePurchaseSproc(migrationBuilder);
            ViewsHelper.CreateCartRecordWithProductInfoView(migrationBuilder);
            ViewsHelper.CreateOrderDetailWithProductInfoView(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            FunctionsHelper.DropOrderTotalFunction(migrationBuilder);
            SprocsHelper.DropPurchaseSproc(migrationBuilder);
            ViewsHelper.DropCartRecordWithProductInfoView(migrationBuilder);
            ViewsHelper.DropOrderDetailWithProductInfoView(migrationBuilder);
        }
    }
}
