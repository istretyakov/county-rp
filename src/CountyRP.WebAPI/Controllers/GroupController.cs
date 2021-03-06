﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using CountyRP.Models;
using CountyRP.WebAPI.DbContexts;
using CountyRP.WebAPI.Models.ViewModels;

namespace CountyRP.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupController : ControllerBase 
    {
        private GroupContext _groupContext;

        public GroupController(GroupContext groupContext)
        {
            _groupContext = groupContext;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Group), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Group group)
        {
            var result = CheckParams(group);
            if (result != null)
                return result;

            var isGroupExisted = await _groupContext.Groups
                .AnyAsync(g => g.Id == group.Id);

            if (!isGroupExisted)
                return BadRequest($"Группа с ID {group.Id} уже существует");

            var groupDAO = MapToDAO(group);

            await _groupContext.Groups.AddAsync(groupDAO);
            await _groupContext.SaveChangesAsync();

            return Created("", group);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Group), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            var groupDAO = await _groupContext.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);

            if (groupDAO == null)
                return NotFound($"Группа с ID {id} не найдена");

            return Ok(
                MapToModel(groupDAO)
            );
        }

        [HttpGet]
        [ProducesResponseType(typeof(Group[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var groupsDAO = await _groupContext.Groups
                .AsNoTracking()
                .ToArrayAsync();

            return Ok(
                groupsDAO
                    .Select(g => MapToModel(g))
            );
        }

        [HttpGet("FilterBy")]
        [ProducesResponseType(typeof(FilteredModels<Group>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FilterBy(int page, int count, string id, string name)
        {
            if (page < 1)
                return BadRequest("Номер страницы групп не может быть меньше 1");

            if (count < 1 || count > 50)
                return BadRequest("Количество групп на одной странице должно быть от 1 до 50");

            IQueryable<DAO.Group> query = _groupContext.Groups;
            if (!string.IsNullOrWhiteSpace(id))
                query = query.Where(g => g.Id.Contains(id));
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(g => g.Name.Contains(name));

            int allAmount = await query.CountAsync();
            int maxPage = (allAmount % count == 0) ? allAmount / count : allAmount / count + 1;
            if (page > maxPage && maxPage > 0)
                page = maxPage;

            var choosenGroups = await query
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToListAsync();

            return Ok(new FilteredModels<Group>
            {
                Items = choosenGroups
                    .Select(g => MapToModel(g))
                    .ToList(),
                AllAmount = allAmount,
                Page = page,
                MaxPage = maxPage
            });
        }

        [HttpGet("Count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCount()
        {
            int count = await _groupContext.Groups
                .CountAsync();

            return Ok(count);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Group), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit(string id, [FromBody] Group group)
        {
            if (id != group.Id)
                return BadRequest($"Указанный ID {id} не соответствует ID группы {group.Id}");

            var isGroupExisted = await _groupContext.Groups
                .AsNoTracking()
                .AnyAsync(g => g.Id == id);

            if (!isGroupExisted)
                return NotFound($"Группа с ID {id} не найдена");

            var result = CheckParams(group);
            if (result != null)
                return result;

            var groupDAO = MapToDAO(group);

            _groupContext.Groups.Update(groupDAO);
            await _groupContext.SaveChangesAsync();

            return Ok(group);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            var groupDAO = await _groupContext.Groups
                .FirstOrDefaultAsync(g => g.Id == id);

            if (groupDAO == null)
                return NotFound($"Группа с ID {id} не найдена");

            _groupContext.Groups.Remove(groupDAO);
            await _groupContext.SaveChangesAsync();

            return Ok();
        }

        private IActionResult CheckParams(Group group)
        {
            TrimParams(group);

            if (group.Id == null || group.Id.Length < 3 || group.Id.Length > 16)
                return BadRequest("ID группы должно состоять от 3 до 16 символов");

            if (group.Name == null || group.Name.Length < 3 || group.Name.Length > 32)
                return BadRequest("Название группы должно состоять от 3 до 32 символов");

            if (group.Color == null || !System.Text.RegularExpressions.Regex.IsMatch(group.Color, "^[0-9a-fA-F]{6}$"))
                return BadRequest("Цвет должна состоять только из следующих символов: 0-9, A-F");

            return null;
        }

        private void TrimParams(Group group)
        {
            group.Id = group.Id?.Trim();
            group.Name = group.Name?.Trim();
            group.Color = group.Color?.Trim();
        }

        private DAO.Group MapToDAO(Group group)
        {
            return new DAO.Group
            {
                Id = group.Id,
                Name = group.Name,
                Color = group.Color,
                AdminPanel = group.AdminPanel
            };
        }

        private Group MapToModel(DAO.Group group)
        {
            return new Group
            {
                Id = group.Id,
                Name = group.Name,
                Color = group.Color,
                AdminPanel = group.AdminPanel
            };
        }
    }
}
