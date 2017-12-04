using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Models.CodeAction;

namespace OmniSharp.LanguageServerProtocol.Handlers
{
    internal class GetCodeActionHandler : ICodeActionHandler
    {
        private readonly Mef.IRequestHandler<GetCodeActionRequest, GetCodeActionsResponse> _getCodeActionHandler;
        private readonly DocumentSelector _documentSelector;
        private CodeActionCapability _capability;

        public GetCodeActionHandler(Mef.IRequestHandler<GetCodeActionRequest, GetCodeActionsResponse> getCodeActionHandler, DocumentSelector documentSelector)
        {
            _getCodeActionHandler = getCodeActionHandler;
            _documentSelector = documentSelector;
        }

        public static IEnumerable<IJsonRpcHandler> Enumerate(RequestHandlers handlers)
        {
            foreach (var (selector, handler) in handlers.OfType<Mef.IRequestHandler<GetCodeActionRequest, GetCodeActionsResponse>>())
                if (handler != null)
                    yield return new GetCodeActionHandler(handler, selector);
        }

        public async Task<CommandContainer> Handle(CodeActionParams request, CancellationToken token)
        {
            var omnisharpRequest = new GetCodeActionRequest
            {
                SelectionStartLine = Convert.ToInt32(request.Range.Start.Line),
                SelectionStartColumn = Convert.ToInt32(request.Range.Start.Character),
                SelectionEndLine = Convert.ToInt32(request.Range.End.Line),
                SelectionEndColumn = Convert.ToInt32(request.Range.End.Line),
                Line = Convert.ToInt32(request.Range.Start.Line),
                Column = Convert.ToInt32(request.Range.Start.Character),
                CodeAction = request.Context.Diagnostics.Count(),
                FileName = Helpers.FromUri(request.TextDocument.Uri)
            };

            var response = await _getCodeActionHandler.Handle(omnisharpRequest);

            var commands = response.CodeActions.Select(action => new Command
            {
                Title = action,
                Name = action
            });

            return new CommandContainer(commands);
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions
            {
                DocumentSelector = _documentSelector
            };
        }

        public void SetCapability(CodeActionCapability capability)
        {
            _capability = capability;
        }
    }
}
