using DataFlowTplController.DataFlow;
using Microsoft.AspNetCore.Mvc;

namespace DataFlowTplController.Controllers;

[ApiController]
[Route("[controller]")]
public class DataFlowController : ControllerBase
{
    private readonly ILogger<DataFlowController> _logger;
    private readonly ParallelAsyncFlow _flow;

    public DataFlowController(ILogger<DataFlowController> logger, ParallelAsyncFlow flow)
    {
        _logger = logger;
        _flow = flow;
    }

    [HttpPost("FlowPost")]
    public async Task Post(int num)
    {
        await _flow.PostMessage(num);
    }

    [HttpPost("FlowPostBatch")]
    public async Task PostBatch(int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            await _flow.PostMessage(i);
        }
    }
    
    [HttpPost("CompleteFlow")]
    public async Task CompleteFlow()
    {
        await _flow.CompleteFlow();
    }
}