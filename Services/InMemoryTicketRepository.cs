using HelpDesk.Results.Models;

namespace HelpDesk.Results.Services;

public class InMemoryTicketRepository : ITicketRepository
{
    private readonly List<Ticket> _tickets;
    private int _nextId = 4;

    public InMemoryTicketRepository()
    {
        _tickets = new List<Ticket>
        {
            new Ticket(1, "Не работает принтер", "Open", 1, DateTime.Now.AddDays(-5)),
            new Ticket(2, "Ошибка входа в систему", "In Progress", 2, DateTime.Now.AddDays(-3)),
            new Ticket(3, "Запрос на установку ПО", "Closed", 3, DateTime.Now.AddDays(-1))
        };
    }

    public IEnumerable<Ticket> GetAll() => _tickets;

    public Ticket? GetById(int id) => _tickets.FirstOrDefault(t => t.Id == id);

    public Ticket Create(string title, int priority)
    {
        var ticket = new Ticket(_nextId++, title, "Open", priority, DateTime.Now);
        _tickets.Add(ticket);
        return ticket;
    }
}