using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Makro.IMS.Services.Api.Configurations
{
    public class GroupingDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var groupedPaths = new Dictionary<string, OpenApiPathItem>();

            foreach (var (path, pathItem) in swaggerDoc.Paths)
            {
                var groupName = ExtractGroupName(path);

                if (!groupedPaths.TryGetValue(groupName, out var existingPathItem))
                {
                    groupedPaths[groupName] = pathItem;
                }
                else
                {
                    // Combine operations from multiple controllers into the same group
                    foreach (var (method, operation) in pathItem.Operations)
                    {
                        existingPathItem.Operations[method] = operation;
                    }
                }
            }

            swaggerDoc.Paths = (OpenApiPaths)groupedPaths;
        }

        private string ExtractGroupName(string path)
        {
            // Implement your custom logic to extract the group name from the path
            // For example, you can use regular expressions or other parsing methods
            // This is just a simple example, adjust it based on your needs
            if (path.StartsWith("/api/first"))
            {
                return "First Group";
            }
            else if (path.StartsWith("/api/second"))
            {
                return "Second Group";
            }

            // Default to a generic group if not matched
            return "Other Group";
        }
    }
}