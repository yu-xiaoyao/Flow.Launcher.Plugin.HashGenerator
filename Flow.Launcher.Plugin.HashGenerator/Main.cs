#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Flow.Launcher.Plugin.HashGenerator
{
    public class HashGenerator : IPlugin, IContextMenu
    {
        public readonly string IconPath = "Images\\HashGenerator.png";

        private PluginInitContext _context;

        public void Init(PluginInitContext context)
        {
            _context = context;
            InnerLogger.SetAsFlowLauncherLogger(context, LoggerLevel.DEBUG);
        }

        public List<Result> Query(Query query)
        {
            //TODO is read from config or input param?
            var format = HashHelper.HashResultFormat.Hex;

            var search = query.Search;
            if (string.IsNullOrEmpty(search))
            {
                var resultList = new List<Result>
                {
                    new()
                    {
                        IcoPath = IconPath,
                        Title = "Input Content",
                        SubTitle = "input content calc hash values",
                        AutoCompleteText = $"{query.ActionKeyword} ",
                        Action = _ =>
                        {
                            _context.API.ChangeQuery($"{query.ActionKeyword} ");
                            return false;
                        }
                    },
                    new()
                    {
                        IcoPath = IconPath,
                        Title = "File Path",
                        SubTitle = "input file path calc hash values",
                        AutoCompleteText = $"{query.ActionKeyword} @",
                        Action = _ =>
                        {
                            _context.API.ChangeQuery($"{query.ActionKeyword} @");
                            return false;
                        }
                    }
                };
                Application.Current.Dispatcher.Invoke(() => _detectClipBoard(query, resultList, format));
                return resultList;
            }

            var results = new List<Result>();

            if (search.StartsWith("@"))
            {
                var filePath = query.FirstSearch[1..];
                if (File.Exists(filePath))
                {
                    var fileBytes = File.ReadAllBytes(filePath);
                    var fileMethods = _buildHashMethods(fileBytes);
                    foreach (var fileMethod in fileMethods)
                    {
                        var result = fileMethod.Hash.Invoke();
                        var hashValue = HashHelper.ToHashString(result, format);
                        results.Add(new()
                        {
                            IcoPath = IconPath,
                            Title = fileMethod.Name,
                            SubTitle = hashValue,
                            CopyText = hashValue,
                            ContextData = result,
                            AutoCompleteText = $"{query.ActionKeyword} @{filePath}",
                            Action = _ =>
                            {
                                _context.API.CopyToClipboard(hashValue, false, false);
                                return true;
                            }
                        });
                    }
                }
            }

            var inputSource = Encoding.UTF8.GetBytes(search);
            var methods = _buildHashMethods(inputSource);
            foreach (var method in methods)
            {
                var result = method.Hash.Invoke();
                var hashValue = HashHelper.ToHashString(result, format);
                results.Add(new()
                {
                    IcoPath = IconPath,
                    Title = method.Name,
                    SubTitle = hashValue,
                    CopyText = hashValue,
                    ContextData = result,
                    AutoCompleteText = $"{query.ActionKeyword} {query.Search}",
                    Action = _ =>
                    {
                        _context.API.CopyToClipboard(hashValue, false, false);
                        return true;
                    }
                });
            }

            return results;
        }

        private List<HashMethod> _buildHashMethods(byte[] inputSource)
        {
            var hashMethods = new List<HashMethod>
            {
                new()
                {
                    Name = "MD5", Hash = () => HashHelper.CalcMd5_32(inputSource)
                },
                new()
                {
                    Name = "SHA1", Hash = () => HashHelper.CalcSha1(inputSource)
                },
                new()
                {
                    Name = "SHA256", Hash = () => HashHelper.CalcSha256(inputSource)
                },
                new()
                {
                    Name = "SHA512", Hash = () => HashHelper.CalcSha512(inputSource)
                },
                new()
                {
                    Name = "MD5_16", Hash = () => HashHelper.CalcMd5_16(inputSource)
                },
                new()
                {
                    Name = "SHA384", Hash = () => HashHelper.CalcSha384(inputSource)
                }
            };

            return hashMethods;
        }

        public List<Result> LoadContextMenus(Result selectedResult)
        {
            var contextData = selectedResult.ContextData;
            if (contextData is not byte[] bytes || bytes.Length == 0)
                return new List<Result>();

            var hexLower = HashHelper.ToHashString(bytes, HashHelper.HashResultFormat.Hex);
            var hexUpper = HashHelper.ToHashString(bytes, HashHelper.HashResultFormat.Hex_Upper);
            var base64 = HashHelper.ToHashString(bytes, HashHelper.HashResultFormat.Base64);

            return new List<Result>()
            {
                new()
                {
                    IcoPath = IconPath,
                    Title = "HEX Lower",
                    SubTitle = hexLower,
                    CopyText = hexLower,
                    Action = _ =>
                    {
                        _context.API.CopyToClipboard(hexLower, false, false);
                        return true;
                    }
                },
                new()
                {
                    IcoPath = IconPath,
                    Title = "Hex Upper",
                    SubTitle = hexUpper,
                    CopyText = hexUpper,
                    Action = _ =>
                    {
                        _context.API.CopyToClipboard(hexUpper, false, false);
                        return true;
                    }
                },
                new()
                {
                    IcoPath = IconPath,
                    Title = "Base64",
                    SubTitle = base64,
                    CopyText = base64,
                    Action = _ =>
                    {
                        _context.API.CopyToClipboard(base64, false, false);
                        return true;
                    }
                }
            };
        }

        private void _detectClipBoard(Query query, List<Result> resultList, HashHelper.HashResultFormat format)
        {
            if (Clipboard.ContainsText())
            {
                var clipboardText = Clipboard.GetText();
                var value = Encoding.UTF8.GetBytes(clipboardText);
                var methods = _buildHashMethods(value);

                var displayText = clipboardText.Replace("\r\n", "  ").Replace("\n", "  ");
                var title = $"Clipboard Text:  {displayText}";

                foreach (var hashMethod in methods)
                {
                    var result = hashMethod.Hash.Invoke();
                    var hashValue = HashHelper.ToHashString(result, format);

                    resultList.Add(new()
                    {
                        IcoPath = IconPath,
                        Title = $"{hashMethod.Name} - {title}",
                        SubTitle = hashValue,
                        CopyText = hashValue,
                        ContextData = result,
                        AutoCompleteText = $"{query.ActionKeyword}",
                        Action = _ =>
                        {
                            _context.API.CopyToClipboard(hashValue, false, false);
                            return true;
                        }
                    });
                }
            }
            else if (Clipboard.ContainsFileDropList())
            {
                var fileDropList = Clipboard.GetFileDropList();

                for (var i = 0; i < fileDropList.Count; i++)
                {
                    var filePath = fileDropList[i];

                    if (string.IsNullOrEmpty(filePath)) continue;

                    var title = $"Clipboard File {i + 1}: {filePath}";
                    try
                    {
                        var fileBytes = File.ReadAllBytes(filePath);
                        var methods = _buildHashMethods(fileBytes);
                        foreach (var hashMethod in methods)
                        {
                            var result = hashMethod.Hash.Invoke();
                            var hashValue = HashHelper.ToHashString(result, format);

                            resultList.Add(new()
                            {
                                IcoPath = IconPath,
                                Title = $"{hashMethod.Name} - {title}",
                                SubTitle = hashValue,
                                CopyText = hashValue,
                                ContextData = result,
                                AutoCompleteText = $"{query.ActionKeyword} @{filePath}",
                                Action = _ =>
                                {
                                    _context.API.CopyToClipboard(hashValue, false, false);
                                    return true;
                                }
                            });
                        }
                    }
                    catch (Exception _)
                    {
                    }
                }
            }
        }
    }

    class HashMethod
    {
        public string Name;
        public Func<byte[]> Hash;
    }
}