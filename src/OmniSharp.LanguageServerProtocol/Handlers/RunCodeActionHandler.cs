using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Models.CodeAction;
using static OmniSharp.LanguageServerProtocol.Helpers;
using GetCodeActionsResponse = OmniSharp.Models.V2.GetCodeActionsResponse;
using RunCodeActionRequest = OmniSharp.Models.V2.RunCodeActionRequest;
using RunCodeActionResponse = OmniSharp.Models.V2.RunCodeActionResponse;

namespace OmniSharp.LanguageServerProtocol.Handlers
{
    /// <summary>
    /// This needs to apply work space edits in order to update the current working document.
    /// </summary>
    /// <seealso cref="OmniSharp.Extensions.LanguageServer.Protocol.IExecuteCommandHandler" />
    internal class RunCodeActionHandler : IExecuteCommandHandler
    {
        private readonly Mef.IRequestHandler<RunCodeActionRequest, RunCodeActionResponse> _runCodeActionHandler;
        private readonly Mef.IRequestHandler<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse> _applyWorkspaceEditHandler;
        private ExecuteCommandCapability _capability;

        public RunCodeActionHandler(Mef.IRequestHandler<RunCodeActionRequest, RunCodeActionResponse> runCodeActionHandler,
            Mef.IRequestHandler<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse> applyWorkspaceEditHandler)
        {
            _runCodeActionHandler = runCodeActionHandler;
            _applyWorkspaceEditHandler = applyWorkspaceEditHandler;
        }

        public static IEnumerable<IJsonRpcHandler> Enumerate(RequestHandlers handlers)
        {
            foreach (var (selector, codeActionhandler, applyWorkspaceHandler) in handlers.OfType<Mef.IRequestHandler<RunCodeActionRequest, RunCodeActionResponse>, Mef.IRequestHandler<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse>>())
                if (applyWorkspaceHandler != null)
                    yield return new RunCodeActionHandler(codeActionhandler, applyWorkspaceHandler);
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

            var changes = response.Changes.ToDictionary(x => ToUri(x.FileName),
                x => new TextEdit
                {
                    NewText = x.ModificationType.ToString()
                });

            var workspaceEditRequest = response.Changes.Select(x => new ApplyWorkspaceEditParams { Edit = new WorkspaceEdit { Changes = changes } }).FirstOrDefault();

            await _applyWorkspaceEditHandler.Handle(workspaceEditRequest);

            await Task.CompletedTask;
        }

        public ExecuteCommandRegistrationOptions GetRegistrationOptions()
        {
            return new ExecuteCommandRegistrationOptions
            {
                Commands = new Container<string>()
            };
        }

        public void SetCapability(ExecuteCommandCapability capability)
        {
            _capability = capability;
        }
    }
}
