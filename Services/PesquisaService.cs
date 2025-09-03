using Microsoft.EntityFrameworkCore;
using IbgeStats.Data;
using IbgeStats.Models;

namespace IbgeStats.Services
{
    public class PesquisaService : IPesquisaService
    {
        private readonly AppDbContext _context;

        public PesquisaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pesquisa>> GetAllPesquisasAsync()
        {
            return await _context.Pesquisas
                .Include(p => p.Indicadores)
                .Include(p => p.Periodos)
                .Include(p => p.Stats)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<Pesquisa?> GetPesquisaByIdAsync(int id)
        {
            return await _context.Pesquisas
                .Include(p => p.Indicadores)
                .Include(p => p.Periodos)
                .Include(p => p.Stats)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pesquisa> CreatePesquisaAsync(Pesquisa pesquisa)
        {
            pesquisa.CreatedAt = DateTime.UtcNow;
            pesquisa.LastUpdated = DateTime.UtcNow;

            _context.Pesquisas.Add(pesquisa);
            await _context.SaveChangesAsync();

            return pesquisa;
        }

        public async Task<Pesquisa?> UpdatePesquisaAsync(int id, Pesquisa pesquisa)
        {
            var existingPesquisa = await _context.Pesquisas.FindAsync(id);
            if (existingPesquisa == null)
                return null;

            existingPesquisa.Nome = pesquisa.Nome;
            existingPesquisa.Descricao = pesquisa.Descricao;
            existingPesquisa.Contexto = pesquisa.Contexto;
            existingPesquisa.Categoria = pesquisa.Categoria;
            existingPesquisa.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingPesquisa;
        }

        public async Task<bool> DeletePesquisaAsync(int id)
        {
            var pesquisa = await _context.Pesquisas.FindAsync(id);
            if (pesquisa == null)
                return false;

            _context.Pesquisas.Remove(pesquisa);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Pesquisa>> GetPesquisasByCategoria(string categoria)
        {
            return await _context.Pesquisas
                .Where(p => p.Categoria.ToLower() == categoria.ToLower())
                .Include(p => p.Stats)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }
    }
}