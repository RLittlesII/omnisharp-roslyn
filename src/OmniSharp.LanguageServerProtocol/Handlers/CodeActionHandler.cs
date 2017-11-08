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
    public class CodeActionHandler : ICodeActionHandler
    {
        private readonly IRequestHandler<GetCodeActionRequest, GetCodeActionsResponse> _getCodeActionRequestHandler;

        public CodeActionHandler(IRequestHandler<GetCodeActionRequest, GetCodeActionsResponse> getCodeActionRequestHandler)
        {
            _getCodeActionRequestHandler = getCodeActionRequestHandler;
        }

        public async Task<CommandContainer> Handle(CodeActionParams request, CancellationToken token)
        {
            var diagnostics = request.Context.Diagnostics.Select(x => new Diagnostic
            {
                Code = x.Code,
                Message = x.Message,
                Range = x.Range,
                Severity = x.Severity,
                Source = x.Source
            });

            var omnisharpRequest = new GetCodeActionRequest
            {
                FileName = Helpers.FromUri(request.TextDocument.Uri)
            };
            var omnisharpResponse = await _getCodeActionRequestHandler.Handle(new GetCodeActionRequest(), token);

            return new CommandContainer();
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions()
        {
            throw new NotImplementedException();
        }

        public void SetCapability(CodeActionCapability capability)
        {
            throw new NotImplementedException();
        }
    }
}
