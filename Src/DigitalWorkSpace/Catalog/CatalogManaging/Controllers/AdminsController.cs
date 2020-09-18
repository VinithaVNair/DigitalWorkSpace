using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using CatalogManaging.Core.Contracts;
using CatalogManaging.Core.Model;
using CatalogManaging.Core.Model.CatalogAggregate;
using CatalogManaging.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CatalogManaging.Controllers
{
    [Route("catalogs/{catalogId}/admins")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class AdminsController : ControllerBase
    {
        private readonly ICardEventHandler _cardEventHandler;
        private readonly ICatalogRepository _catalogRepository;
        private readonly ILogger<AdminsController> _logger;
        public AdminsController(ICatalogRepository catalogRepository, ICardEventHandler cardEventHandler, ILogger<AdminsController> logger)
        {
            _catalogRepository = catalogRepository;
            _cardEventHandler = cardEventHandler;
            _logger = logger;
        }

        /// <summary>
        /// Add user as admin to the given catalog
        /// Only admin of the catalog can perform this action
        /// </summary>
        /// <param name="adminAddDto">Detail of the user to be added and admin who is adding the user</param>
        /// <param name="catalogId">Catalog id to which the user needs to be added as admin</param>
        /// <returns>An Action result of type Admin</returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult<Admin> AddAdmin([FromBody] AdminDto adminAddDto, int catalogId)
        {
            _logger.LogInformation("Adding user {userId} as admin for catalog {catalogId} initiated", adminAddDto.UserId, catalogId);
            var catalog = Catalog.GetExistingCatalog(catalogId, _catalogRepository, _cardEventHandler);
            if (catalog == null)
            {
                return NotFound();
            }
            var admin =catalog.AddAdmin(adminAddDto.AdminId, adminAddDto.UserId);
            if(admin==null)
            {
                return Forbid();
            }
            return Ok(admin);
        }

        /// <summary>
        /// Get all admins of the given catalog id
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns>An Action result with the list of Admin</returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<Admin>> GetAdmin(int catalogId)
        {
            var catalog = Catalog.GetExistingCatalog(catalogId, _catalogRepository, _cardEventHandler);
            if (catalog == null)
            {
                return NotFound();
            }
            var admins = catalog.GetAllAdmin();
            return Ok(admins);
        }

        /// <summary>
        /// Delete admin from the catalog 
        /// Only admin of the catalog can perform this action
        /// </summary>
        /// <param name="adminDeleteDto">Details of the user who needs to be deleted and the admin who is performing the deletion</param>
        /// <param name="catalogId"></param>
        /// <returns>Action result of type boolean</returns>
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult<bool> DeleteAdmin([FromBody] AdminDto adminDeleteDto, int catalogId)
        {
            _logger.LogInformation("Deleting user {userId} from admin group for catalog {catalogId} initiated", adminDeleteDto.UserId, catalogId);
            var catalog = Catalog.GetExistingCatalog(catalogId, _catalogRepository, _cardEventHandler);
            if (catalog == null)
            {
                return NotFound();
            }

            var isDeleted=catalog.RemoveAdmin(adminDeleteDto.AdminId, adminDeleteDto.UserId);

            if(!isDeleted)
            {
                return Forbid();
            }
            return Ok(isDeleted);
        }
    }
}
