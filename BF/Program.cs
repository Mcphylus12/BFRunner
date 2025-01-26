// See https://aka.ms/new-console-template for more information

using BF;

var file = File.ReadAllText(args[0]);

var tokens = new Tokeniser().Process(file);
var model = new CodeModel(tokens);
model.Dump();
var host = new Host(true);
host.Run(model.Root);
