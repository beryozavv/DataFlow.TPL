using System.Threading.Tasks.Dataflow;

namespace DataFlowTplController.DataFlow;

public class ParallelAsyncFlow
{
    // Create a BufferBlock to act as the initial queue for messages
    private readonly BufferBlock<int> _bufferBlock = new(new DataflowBlockOptions{EnsureOrdered = true});
    private readonly ActionBlock<string> _actionBlockFinal = new(text =>
    {
        Console.WriteLine(DateTime.Now.ToString("O") + " - " + text);
    });

    public void InitFlow()
    {
        // Create a TransformBlock to process messages asynchronously and in parallel
        var transformBlock = new TransformBlock<int, string>(async number =>
        {
            Console.WriteLine($"Post {number} - {DateTime.Now.ToString("O")}");
            await Task.Delay(1000); // Simulate async work
            return $"Operation: {number} ";
        }, new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 8,
            EnsureOrdered = true,
            MaxDegreeOfParallelism = 4 // Process up to 4 messages in parallel
        });

        var transformBlockSimple = new TransformBlock<int, string>(async number =>
        {
            Console.WriteLine($"Post Simple {number} - {DateTime.Now.ToString("O")}");
            await Task.Delay(100); // Simulate async work
            return $"Simple operation: {number} ";
        }, new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 16,
            EnsureOrdered = true,
            MaxDegreeOfParallelism = 8 // Process up to 4 messages in parallel
        });

        // Create an ActionBlock to process the transformed messages in parallel
        var transformBlock2 = new TransformBlock<string,string>(async text =>
        {
            await Task.Delay(2000); // Simulate async work
            return text;
        }, new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 4,
            EnsureOrdered = true,
            MaxDegreeOfParallelism = 4 // Process up to 4 messages in parallel
        });
        
        var transformBlockSimple2 = new TransformBlock<string,string>(async text =>
        {
            await Task.Delay(700); // Simulate async work
            return text;
        }, new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 8,
            EnsureOrdered = true,
            
            MaxDegreeOfParallelism = 8 // Process up to 4 messages in parallel
        });
        
        // todo убрать
        // ActionBlock<string> actionBlockFinalTemp = new(text =>
        // {
        //     Console.WriteLine(DateTime.Now.ToString("O") + " - " + text);
        // });

        // Link the blocks together, ensuring completion is propagated
        var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
        _bufferBlock.LinkTo(transformBlock, linkOptions,
            num => num % 2 == 0);
        _bufferBlock.LinkTo(transformBlockSimple, linkOptions,
            num => num % 2 != 0);
        transformBlock.LinkTo(transformBlock2, linkOptions);
        transformBlockSimple.LinkTo(transformBlockSimple2, linkOptions);
        transformBlock2.LinkTo(_actionBlockFinal, linkOptions);
        transformBlockSimple2.LinkTo(_actionBlockFinal, linkOptions);
    }

    public async Task PostMessage(int num)
    {
        await _bufferBlock.SendAsync(num);
    }

    public async Task CompleteFlow()
    {
        // Signal completion to the BufferBlock
        _bufferBlock.Complete();

        // Await the completion of the last block in the pipeline
        await _actionBlockFinal.Completion;
    }
}