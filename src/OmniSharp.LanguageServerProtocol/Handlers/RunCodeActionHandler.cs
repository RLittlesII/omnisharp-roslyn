using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Mef;
using OmniSharp.Models.V2;
using static OmniSharp.LanguageServerProtocol.Helpers;

namespace OmniSharp.LanguageServerProtocol.Handlers
{
    /// <summary>
    /// This needs to apply work space edits in order to update the current working document.
    /// </summary>
    /// <seealso cref="OmniSharp.Extensions.LanguageServer.Protocol.IExecuteCommandHandler" />
    internal class RunCodeActionHandler : IExecuteCommandHandler
    {
        private readonly IRequestHandler<RunCodeActionRequest, RunCodeActionResponse> _runCodeActionHandler;
        private readonly IRequestHandler<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse> _applyWorkspaceEditHandler;
        private ExecuteCommandCapability _capability;

        public RunCodeActionHandler(
            IRequestHandler<RunCodeActionRequest, RunCodeActionResponse> runCodeActionHandler,
            IRequestHandler<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse> applyWorkspaceEditHandler)
        {
            _runCodeActionHandler = runCodeActionHandler;
            _applyWorkspaceEditHandler = applyWorkspaceEditHandler;
        }

        public async Task Handle(ExecuteCommandParams request, CancellationToken token)
        {
            var range = request.Arguments[1].ToObject<Extensions.LanguageServer.Protocol.Models.Range>();

            var omnisharpRequest = new RunCodeActionRequest
            {
                Identifier = request.Arguments[0].ToObject<string>(),
                Selection = ToRange(range)
            };

            var response = await _runCodeActionHandler.Handle(omnisharpRequest);

            var workspaceEditRequest = response.Changes.Select(x => new ApplyWorkspaceEditParams { Edit = new WorkspaceEdit() }).FirstOrDefault();

            await _applyWorkspaceEditHandler.Handle(workspaceEditRequest);

            await Task.CompletedTask;
        }

        public ExecuteCommandRegistrationOptions GetRegistrationOptions()
        {
            return new ExecuteCommandRegistrationOptions
            {
            };
        }

        public void SetCapability(ExecuteCommandCapability capability)
        {
            _capability = capability;
        }
    }
}
