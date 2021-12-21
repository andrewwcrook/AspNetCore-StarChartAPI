using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

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

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObjet)
        {
            var oldCelestialObject = _context.CelestialObjects
                .FirstOrDefault(c => c.Id == id);

            if (oldCelestialObject == null) return NotFound();

            oldCelestialObject.Name = celestialObjet.Name;
            oldCelestialObject.OrbitalPeriod = celestialObjet.OrbitalPeriod;
            oldCelestialObject.OrbitedObjectId = celestialObjet.OrbitedObjectId;

            _context.CelestialObjects.Update(oldCelestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects
                .FirstOrDefault(c => c.Id == id);

            if (celestialObject == null) return NotFound();

            celestialObject.Name = name;

            _context.CelestialObjects.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects
                .Where(c => c.Id == id || (c.OrbitedObjectId.HasValue && c.OrbitedObjectId.Value == id))
                .ToList();

            if (celestialObjects.Count == 0) return NotFound();

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
