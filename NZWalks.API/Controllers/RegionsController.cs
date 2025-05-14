using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper; //ovo je iz paketa automapper!
        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalksDbContext dbContext,IRegionRepository regionRepository,IMapper mapper,ILogger<RegionsController> logger)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        


        [HttpGet]
        //[Authorize(Roles ="Reader")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                //throw new Exception("This is a custom exception!");

                //Get Data From Database-Domain models


                var regions = await regionRepository.GetAllAsync();
                logger.LogInformation($"Finished GetAllRegions request with data: {JsonSerializer.Serialize(regions)}");

                //Map Domain Models to DTOs

                //var regionsDto = new List<RegionDto>();
                //foreach (var region in regions)
                //{
                //    regionsDto.Add(new RegionDto()
                //    {
                //        Id = region.Id,
                //        Name = region.Name,
                //        Code = region.Code,
                //        RegionImageUrl = region.RegionImageUrl


                //    }
                //        );

                //}

                var regionsDto = mapper.Map<List<RegionDto>>(regions);

                //return DTOs
                return Ok(regionsDto);

            }
            catch (Exception ex)
            {

                logger.LogError(ex, ex.Message);
                throw;

            }
            


            
        }

        [HttpGet]
        [Route("{id:guid}")]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute]Guid id)
        {
            //var region=dbContext.Regions.Find(id);
            var region = await regionRepository.GetByIdAsync(id);

            if (region == null) { 
            
                return NotFound();
            }
            //pakujemo dobijene podatke u DTO
            //var regionDto = new RegionDto()
            //{
            //    Id = region.Id,
            //    Name = region.Name,
            //    Code = region.Code,
            //    RegionImageUrl = region.RegionImageUrl

            //};

            var regionDto = mapper.Map<RegionDto>(region);

            return Ok(regionDto);
        }

        //POST To Create New Region
        [HttpPost]
        [ValidateModel]
        //[Authorize(Roles ="Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            //Map or Convert DTO to Domain Model

            //var regionDomainModel = new Region()
            //{
            //    Code = addRegionRequestDto.Code,
            //    Name = addRegionRequestDto.Name,
            //    RegionImageUrl = addRegionRequestDto.RegionImageUrl
            //};

            //Checking validation
            
            


            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

            //Use Domain Model to create Region
           regionDomainModel= await regionRepository.CreateAsync(regionDomainModel);


            //Map domain model back to DTO

            //var regionDto = new RegionDto()
            //{
            //    Id=regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name=regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl

            //};

            var regionDto = mapper.Map<RegionDto>(regionDomainModel);
            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
            
            
        }

        //Update Region
        //PUT:{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody]UpdateRegionRequestDto updateRegionRequestDto)
        {
            //var regionDomainModel = new Region()
            //{
            //    Code = updateRegionRequestDto.Code,
            //    Name = updateRegionRequestDto.Name,
            //    RegionImageUrl = updateRegionRequestDto.RegionImageUrl


            //};
            

            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);
            
            //Check if region exists
            regionDomainModel=await regionRepository.UpdateAsync(id,regionDomainModel);
            if (regionDomainModel == null)
            {
                return NotFound();
            }




            //Convert DomainModel to DTO so we can represent it to client
            //var regionDto = new RegionDto()
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl


            //};
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);
            
            


        }


        //Delete Region

        [HttpDelete]
        [Route("{id:guid}")]
        //[Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await regionRepository.DeleteAsync(id);
            if(regionDomainModel == null)
            {
                return NotFound();
            };
            

            //Delete region

            //optional returning deleted region
            //Map deleted region model to dto
            //var regionDto = new RegionDto()
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl


            //};
            var regionDto=mapper.Map<RegionDto>(regionDomainModel);



            return Ok(regionDto);



        }


    }
}
