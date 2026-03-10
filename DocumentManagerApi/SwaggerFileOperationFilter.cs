using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DocumentManagerApi;

public class SwaggerFileOperationFilter : IOperationFilter  
{  
    public void Apply(OpenApiOperation operation, OperationFilterContext context)  
    {  
        var fileUploadMime = "multipart/form-data";  
        if (operation.RequestBody == null || !operation.RequestBody.Content.Any(x => x.Key.Equals(fileUploadMime, StringComparison.InvariantCultureIgnoreCase)))  
            return;  
      
        var fileParams = context.MethodInfo.GetParameters().Where(p => p.ParameterType == typeof(IFormFile));
        
        var schema = operation.RequestBody.Content[fileUploadMime].Schema;
        
        foreach (var param in fileParams)
        {
            schema.Properties[param.Name!] = new OpenApiSchema()  
            {  
                Type = JsonSchemaType.String,  
                Format = "binary"  
            };
        }  
    }  
}