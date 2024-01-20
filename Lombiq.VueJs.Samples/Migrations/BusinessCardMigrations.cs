using Lombiq.VueJs.Samples.Models;
using OrchardCore.ContentFields.Settings;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Samples.Migrations;

public class BusinessCardMigrations(IContentDefinitionManager contentDefinitionManager) : DataMigration
{
    public async Task<int> CreateAsync()
    {
        var businessCardPart = await contentDefinitionManager.AlterPartDefinitionAsync<BusinessCard>(partBuilder => partBuilder
            .WithField(model => model.FirstName, field => field
                .WithDisplayName("First name")
                .WithSettings(new TextFieldSettings { Required = true }))
            .WithField(model => model.LastName, field => field
                .WithDisplayName("Last name")
                .WithSettings(new TextFieldSettings { Required = true }))
            .WithField(model => model.Email, field => field
                .WithSettings(new TextFieldSettings { Required = true }))
            .WithField(model => model.Phone, field => field
                .WithSettings(new TextFieldSettings { Required = true })));

        await contentDefinitionManager.AlterTypeDefinitionAsync(nameof(BusinessCard), typeBuilder => typeBuilder
            .DisplayedAs("Business card")
            .Creatable()
            .Listable()
            .Draftable()
            .Versionable()
            .WithPart(businessCardPart));

        return 1;
    }
}
