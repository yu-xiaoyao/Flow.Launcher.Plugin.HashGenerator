#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Launcher.Plugin.HashGenerator
{
    public class HashGenerator : IPlugin
    {
        public readonly string IconPath = "Images\\hash-generator.png";

        private PluginInitContext _context;

        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            var search = query.Search;
            if (string.IsNullOrEmpty(search))
            {
                return new List<Result>
                {
                    new()
                    {
                        IcoPath = IconPath,
                        SubTitle = "input content calc hash values"
                    }
                };
            }

            var inputSource = Encoding.UTF8.GetBytes(search);
            //TODO is read from config or input param?
            var format = HashHelper.HashResultFormat.Hex;

            var hashMethods = new List<HashMetohd>
            {
                new()
                {
                    Name = "MD5", Hash = () => HashHelper.CalcMd5_32(inputSource, format)
                },
                new()
                {
                    Name = "SHA1", Hash = () => HashHelper.CalcSha1(inputSource, format)
                },
                new()
                {
                    Name = "SHA256", Hash = () => HashHelper.CalcSha256(inputSource, format)
                },
                new()
                {
                    Name = "SHA512", Hash = () => HashHelper.CalcSha512(inputSource, format)
                },
                new()
                {
                    Name = "MD5_16", Hash = () => HashHelper.CalcMd5_16(inputSource, format)
                },
                new()
                {
                    Name = "SHA384", Hash = () => HashHelper.CalcSha384(inputSource, format)
                }
            };

            var results = new List<Result>();
            foreach (var hashMethod in hashMethods)
            {
                var result = hashMethod.Hash.Invoke();
                if (result == null) continue;
                results.Add(new()
                {
                    IcoPath = IconPath,
                    Title = hashMethod.Name,
                    SubTitle = result,
                    CopyText = result,
                    Action = _ =>
                    {
                        _context.API.CopyToClipboard(result, false, false);
                        return true;
                    }
                });
            }

            return results;
        }
    }

    class HashMetohd
    {
        public string Name;
        public Func<string?> Hash;
    }
}