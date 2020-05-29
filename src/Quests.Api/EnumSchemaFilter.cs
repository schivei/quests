using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using Microsoft.OpenApi.Any;
using System.Runtime.Serialization;
using Quests.DomainModels;

namespace Quests.Api
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                var enumType = context.Type;
                var memberInfos = enumType.GetMembers();

                var valueAttributes =
                      memberInfos.Select(i =>
                        i.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                            .FirstOrDefault() as EnumMemberAttribute)
                      .Where(x => x != null && x.Value != null)
                      .ToList();

                if (valueAttributes.Any())
                {
                    model.Enum.Clear();
                    valueAttributes
                        .ForEach(n => model.Enum.Add(new OpenApiString(n.Value)));
                }
            }
            else if (context.MemberInfo?.Name == nameof(AEntity.DgraphType))
            {
                model.Example =
                model.Default = new OpenApiString(Dgraph4Net.DgraphExtensions.GetDType(context.MemberInfo.ReflectedType.GetConstructors().First().Invoke(null)));
            }
        }
    }
}
