namespace ActiveX_Api.Utils
{
    public static class FileValidation
    {
        public static bool ValidateFile(IFormFile? file)
        {
            //if (productDto.File3DModel.ContentType != "model/gltf-binary") return BadRequest("Invalid file type.");
            if (file == null) return false;
            if(file.Length > 0)
            {
                return true; 
            }
            return false;

        }

        public static string GenerateFileName()
        {
            string randomFileName = Path.GetRandomFileName();
            int dotIndex = randomFileName.LastIndexOf('.');
            string fileName = randomFileName.Substring(0, dotIndex) + ".glb";
            return fileName;
        }
    }
}
