﻿using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
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
    public class LogController : ControllerBase
    {
        private LogContext _logContext;
        private PlayerContext _playerContext;

        public LogController(LogContext logContext, PlayerContext playerContext)
        {
            _logContext = logContext;
            _playerContext = playerContext;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LogUnit), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var logUnitDAO = await _logContext.LogUnits
                .AsNoTracking()
                .FirstOrDefaultAsync(lu => lu.Id == id);

            if (logUnitDAO == null)
                return NotFound($"Лог с ID {id} не найден");

            return Ok(
                MapToModel(logUnitDAO)
            );
        }

        [HttpGet("FilterBy")]
        [ProducesResponseType(typeof(FilteredModels<LogUnit>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FilterBy(int page, int count, string login, string ip, int actionId, string commentPart)
        {
            if (page < 1)
                return BadRequest("Номер страницы логов не может быть меньше 1");

            if (count < 1 || count > 50)
                return BadRequest("Количество логов на одной странице должно быть от 1 до 50");

            IQueryable<DAO.LogUnit> query = _logContext.LogUnits;

            var choosenLogUnits = await query
                    .OrderByDescending(lu => lu.DateTime)
                    .Where(lu => 
                        !string.IsNullOrWhiteSpace(login) ? lu.Login == login : true &&
                        !string.IsNullOrWhiteSpace(ip) ? lu.IP.Contains(ip) : true &&
                        actionId != -1 ? lu.ActionId == (DAO.LogAction)actionId : true &&
                        !string.IsNullOrWhiteSpace(commentPart) ? lu.Comment.Contains(commentPart) : true
                    )
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToListAsync();

            int allAmount = choosenLogUnits.Count;
            int maxPage = (allAmount % count == 0) ? allAmount / count : allAmount / count + 1;
            if (page > maxPage && maxPage > 0)
                page = maxPage;

            return Ok(new FilteredModels<LogUnit>
            {
                Items = choosenLogUnits
                    .Select(lu => MapToModel(lu))
                    .ToList(),
                AllAmount = allAmount,
                Page = page,
                MaxPage = maxPage
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(LogUnit), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] LogUnit logUnit)
        {
            var error = CheckParams(logUnit);
            if (error != null)
                return error;

            var logUnitDAO = MapToDAO(logUnit);
            logUnitDAO.Id = 0;

            await _logContext.LogUnits.AddAsync(logUnitDAO);
            await _logContext.SaveChangesAsync();

            return Created("", MapToModel(logUnitDAO));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(LogUnit), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit(int id, [FromBody] LogUnit logUnit)
        {
            if (logUnit.Id != id)
                return BadRequest($"Указанный ID {id} не соответствует ID {logUnit.Id} лога");

            var isLogUnitExisted = await _logContext.LogUnits
                .AsNoTracking()
                .AnyAsync(lu => lu.Id == id);

            if (!isLogUnitExisted)
                return NotFound($"Лог с ID {id} не найден");

            var error = CheckParams(logUnit);
            if (error != null)
                return error;

            var logUnitDAO = MapToDAO(logUnit);

            _logContext.LogUnits.Update(logUnitDAO);
            await _logContext.SaveChangesAsync();

            return Ok(logUnit);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var logUnitDAO = await _logContext.LogUnits
                .SingleOrDefaultAsync(lu => lu.Id == id);

            if (logUnitDAO == null)
                return NotFound($"Лог с ID {id} не найден");

            _logContext.LogUnits.Remove(logUnitDAO);
            await _logContext.SaveChangesAsync();

            return Ok();
        }

        private DAO.LogUnit MapToDAO(LogUnit lu)
        {
            return new DAO.LogUnit
            {
                Id = lu.Id,
                DateTime = lu.DateTime,
                Login = lu.Login,
                IP = lu.IP,
                ActionId = (DAO.LogAction)lu.ActionId,
                Comment = lu.Comment
            };
        }

        private LogUnit MapToModel(DAO.LogUnit lu)
        {
            return new LogUnit
            {
                Id = lu.Id,
                DateTime = lu.DateTime,
                Login = lu.Login,
                IP = lu.IP,
                ActionId = (LogAction)lu.ActionId,
                Comment = lu.Comment
            };
        }

        private IActionResult CheckParams(LogUnit logUnit)
        {
            if (logUnit.Login.Length < 3 || logUnit.Login.Length > 32)
                return BadRequest("Логин должен быть от 3 до 32 символов");

            if (!string.IsNullOrWhiteSpace(logUnit.IP) && !Regex.IsMatch(logUnit.IP, "^[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}$"))
                return BadRequest("IP должен быть в формате 255.255.255.255");

            return null;
        }
    }
}
