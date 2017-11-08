using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Models;
using OmniSharp.Models.CodeAction;
using OmniSharp.Models.FindUsages;

namespace OmniSharp.LanguageServerProtocol.Handlers
{
    public class CodeLensResolverHandler : ICodeLensResolveHandler
    {
        private readonly IRequestHandler<FindUsagesRequest, QuickFixResponse> _findUsageRequestHandler;

        public CodeLensResolverHandler(IRequestHandler<FindUsagesRequest, QuickFixResponse> findUsageRequestHandler)
        {
            _findUsageRequestHandler = findUsageRequestHandler;
        }

        public async Task<CodeLens> Handle(CodeLens request, CancellationToken token)
        {
            var omnisharpRequest = new FindUsagesRequest
            {
            };

            var omnisharpResponse = await _findUsageRequestHandler.Handle(omnisharpRequest, token);

            var lens = new CodeLens
            {
            };

            return lens;
        }
    }
}
