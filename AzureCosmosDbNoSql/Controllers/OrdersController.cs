using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzureCosmosDbNoSql.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController(OrdersRepository ordersRepository) : ControllerBase
{
    private readonly OrdersRepository _ordersRepository = ordersRepository;

    [HttpGet]
    public async Task<ActionResult<Order>> Get(Guid userId, Guid orderId, CancellationToken cancellationToken)
    {
        return Ok(await _ordersRepository.GetAsync(userId, orderId, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Order order, CancellationToken cancellationToken)
    {
        await _ordersRepository.AddAsync(order, cancellationToken);
        await _ordersRepository.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> Put([FromBody] Order order, CancellationToken cancellationToken)
    {
        _ordersRepository.Update(order);
        await _ordersRepository.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(Guid userId, Guid orderId, CancellationToken cancellationToken)
    {
        await _ordersRepository.DeleteAsync(userId, orderId, cancellationToken);
        await _ordersRepository.SaveChangesAsync(cancellationToken);
        return Ok();
    }
}

public class OrdersRepository(AppDbContext context)
{
    protected readonly AppDbContext _context = context;
    protected readonly DbSet<Order> _dbset = context.Set<Order>();

    public async Task<Order> GetAsync(Guid userId, Guid orderId, CancellationToken cancellationToken)
    {
        return await _dbset.FirstAsync(e => e.UserId == userId && e.OrderId == orderId, cancellationToken);
    }

    public async Task AddAsync(Order entity, CancellationToken cancellationToken) => await _dbset.AddAsync(entity, cancellationToken);
    public void Update(Order entity) => _dbset.Update(entity);
    public async Task DeleteAsync(Guid userId, Guid orderId, CancellationToken cancellationToken)
    {
        var entity = await GetAsync(userId, orderId, cancellationToken);
        _dbset.Remove(entity);
    }
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}

