﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        //POST: /api/Images/Upload

        [HttpPost]
        [Route("Upload")]

        public async Task<IActionResult> Upload([FromForm]ImageUploadRequestDto request)
        {
             ValidateFileUpload(request);  
  
            if(ModelState.IsValid)
            {
                //Convert DTO to Domain Modele
                var imagesDomainModel = new Image
                {
                    File = request.File,
                    FileExtension = Path.GetExtension(request.File.FileName),
                    FileSizeInBytes = request.File.Length,
                    FileName = request.File.FileName,
                    FileDescription = request.FileDescription,
                };



                //User repository to upload image
                await imageRepository.Upload(imagesDomainModel);
                return Ok(imagesDomainModel);

            }

            return BadRequest(ModelState);

        }

        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            var allowedExtensions = new string[]
            {
                ".jpg",".jpeg",".png"


            };

            if (!allowedExtensions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extension");

            }

            if (request.File.Length > 1048760)
            {
                ModelState.AddModelError("file", "File size more than 10MB, please upload a smaller size file");
            }

        }







    }
}
