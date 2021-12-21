using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

namespace StarChart.Controllers
{
    [ApiController]
    [Route("")]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects
                .FirstOrDefault(c => c.Id == id);

            if (celestialObject == null) 
                return NotFound();

            celestialObject.Satellites = _context.CelestialObjects
                .Where(c => c.OrbitedObjectId.HasValue && c.OrbitedObjectId.Value == celestialObject.Id)
                .ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects
                .Where(c => c.Name.Equals(name))
                .ToList();

            if (celestialObjects.Count == 0)
                return NotFound();

            celestialObjects.ForEach(celestialObject =>
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(c => c.OrbitedObjectId.HasValue && c.OrbitedObjectId.Value == celestialObject.Id)
                    .ToList()
            );

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects
                .ToList();

            celestialObjects.ForEach(celestialObject =>
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(c => c.OrbitedObjectId.HasValue && c.OrbitedObjectId.Value == celestialObject.Id)
                    .ToList()
            );

            return Ok(celestialObjects);
        }
    }
}
