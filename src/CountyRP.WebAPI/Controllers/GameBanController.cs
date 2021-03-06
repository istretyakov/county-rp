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
    public class GameBanController : ControllerBase
    {
        private BanContext _banContext;
        private PlayerContext _playerContext;

        public GameBanController(BanContext banContext, PlayerContext playerContext)
        {
            _banContext = banContext;
            _playerContext = playerContext;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GameBan), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var gameBanDAO = await _banContext.GameBans
                .AsNoTracking()
                .FirstOrDefaultAsync(gb => gb.Id == id);

            if (gameBanDAO == null)
                return NotFound($"Бан в игре с ID {id} не найден");

            return Ok(
                MapToModel(gameBanDAO)
            );
        }

        [HttpGet("FilterBy")]
        [ProducesResponseType(typeof(FilteredModels<GameBan>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FilterBy(int page, int count)
        {
            if (page < 1)
                return BadRequest("Номер страницы банов не может быть меньше 1");

            if (count < 1 || count > 50)
                return BadRequest("Количество банов на одной странице должно быть от 1 до 50");

            IQueryable<DAO.GameBan> query = _banContext.GameBans;

            int allAmount = await query.CountAsync();
            int maxPage = (allAmount % count == 0) ? allAmount / count : allAmount / count + 1;
            if (page > maxPage && maxPage > 0)
                page = maxPage;

            var choosenGameBans = await query
                    .Skip((page - 1) * count)
                    .Take(count)
                    .Select(gb => gb)
                    .ToListAsync();

            return Ok(new FilteredModels<GameBan>
            {
                Items = choosenGameBans
                    .Select(gb => MapToModel(gb))
                    .ToList(),
                AllAmount = allAmount,
                Page = page,
                MaxPage = maxPage
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(GameBan), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] GameBan gameBan)
        {
            var error = await CheckParamsAsync(gameBan);
            if (error != null)
                return error;

            var gameBanDAO = MapToDAO(gameBan);
            gameBanDAO.Id = 0;

            await _banContext.GameBans.AddAsync(gameBanDAO);
            await _banContext.SaveChangesAsync();

            return Created("", MapToModel(gameBanDAO));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(GameBan), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit(int id, [FromBody] GameBan gameBan)
        {
            if (gameBan.Id != id)
                return BadRequest($"Указанный ID {id} не соответствует ID {gameBan.Id} бана в игре");

            var gameBanDAO = await _banContext.GameBans
                .AsNoTracking()
                .FirstOrDefaultAsync(gb => gb.Id == id);

            if (gameBanDAO == null)
                return NotFound($"Бан в игре с ID {id} не найден");

            var error = await CheckParamsAsync(gameBan);
            if (error != null)
                return error;

            gameBanDAO = MapToDAO(gameBan);

            _banContext.GameBans.Update(gameBanDAO);
            await _banContext.SaveChangesAsync();

            return Ok(gameBan);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var gameBanDAO = await _banContext.GameBans
                .FirstOrDefaultAsync(gb => gb.Id == id);

            if (gameBanDAO == null)
                return NotFound($"Бан в игре с ID {id} не найден");

            _banContext.GameBans.Remove(gameBanDAO);
            await _banContext.SaveChangesAsync();

            return Ok();
        }

        private async Task<IActionResult> CheckParamsAsync(GameBan gameBan)
        {
            if (gameBan.PlayerId != 0 && gameBan.PersonId != 0)
                return BadRequest($"Можно забанить только либо игрока, либо персонажа");

            var isPlayerExisted = await _playerContext.Players
                .AnyAsync(p => p.Id == gameBan.PlayerId);

            if (gameBan.PersonId == 0 && !isPlayerExisted)
                return BadRequest($"Забаненный игрок с ID {gameBan.PlayerId} не найден");

            var isPersonExisted = await _playerContext.Persons
                .AnyAsync(p => p.Id == gameBan.PersonId);

            if (gameBan.PlayerId == 0 && !isPersonExisted)
                return BadRequest($"Забаненный персонаж с ID {gameBan.PersonId} не найден");

            var isAdminPlayerExisted = await _playerContext.Players
                .AnyAsync(p => p.Id == gameBan.AdminId);

            if (!isAdminPlayerExisted)
                return BadRequest($"Забанивший игрок с ID {gameBan.AdminId} не найден");

            if (gameBan.StartDateTime > gameBan.FinishDateTime)
                return BadRequest("Дата бана не может быть больше даты окончания бана");

            if (gameBan.Reason.Length < 1 || gameBan.Reason.Length > 96)
                return BadRequest("Причина бана должна быть от 1 до 96 символов");

            if (!string.IsNullOrWhiteSpace(gameBan.IP) && !Regex.IsMatch(gameBan.IP, "^[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}$"))
                return BadRequest("IP должен быть в формате 255.255.255.255");

            return null;
        }

        private DAO.GameBan MapToDAO(GameBan gameBan)
        {
            return new DAO.GameBan
            {
                Id = gameBan.Id,
                PlayerId = gameBan.PlayerId,
                PersonId = gameBan.PersonId,
                AdminId = gameBan.AdminId,
                StartDateTime = gameBan.StartDateTime,
                FinishDateTime = gameBan.FinishDateTime,
                IP = gameBan.IP,
                Reason = gameBan.Reason
            };
        }

        private GameBan MapToModel(DAO.GameBan gameBan)
        {
            return new GameBan
            {
                Id = gameBan.Id,
                PlayerId = gameBan.PlayerId,
                PersonId = gameBan.PersonId,
                AdminId = gameBan.AdminId,
                StartDateTime = gameBan.StartDateTime,
                FinishDateTime = gameBan.FinishDateTime,
                IP = gameBan.IP,
                Reason = gameBan.Reason
            };
        }
    }
}
