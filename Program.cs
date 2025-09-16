using NBitcoin;

namespace Rumpar.Web3Tools2._1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // network: 改为 Network.Main 在主网生成真实地址（小心私钥安全）
            Network network = Network.TestNet;

            // 1) 生成助记词（BIP39）
            var mn = new Mnemonic(Wordlist.English, WordCount.Twelve);
            Console.WriteLine("=== Mnemonic (BIP39) ===");
            Console.WriteLine(mn);
            Console.WriteLine();

            // 2) 得到 ExtKey（2种可选方式）
            // 方式 A（推荐）: 直接从 Mnemonic 得到 ExtKey
            ExtKey masterKey = mn.DeriveExtKey();

            // 方式 B（等价）: 先拿到 seed    ，再 new ExtKey(seed)
            // byte[] seed = mn.DeriveSeed();
            // ExtKey masterKey = new ExtKey(seed);

            // 3) 派生（示例使用 BIP84: m/84'/coinType'/0'）
            uint coinType = network == Network.Main ? 0u : 1u;
            KeyPath account0 = new KeyPath($"84'/{coinType}'/0'");
            ExtKey accountKey = masterKey.Derive(account0);

            // 外部链 change=0，索引 0
            ExtKey extKey0 = accountKey.Derive(new KeyPath("0/0"));

            // 私钥、公钥
            Key privateKey = extKey0.PrivateKey;
            PubKey pubKey = privateKey.PubKey;

            // 私钥导出
            string wif = privateKey.GetWif(network).ToString();
            string privateHex = Convert.ToHexString(privateKey.ToBytes()).ToLowerInvariant();

            Console.WriteLine("=== Private Key ===");
            Console.WriteLine("WIF: " + wif);
            Console.WriteLine("Hex: " + privateHex);
            Console.WriteLine();

            // 常见地址
            var p2pkh = pubKey.GetAddress(ScriptPubKeyType.Legacy, network);
            var p2wpkh = pubKey.GetAddress(ScriptPubKeyType.Segwit, network);
            var p2sh_p2wpkh = pubKey.GetAddress(ScriptPubKeyType.SegwitP2SH, network);

            Console.WriteLine("=== Addresses ===");
            Console.WriteLine("P2PKH (legacy):      " + p2pkh);
            Console.WriteLine("P2WPKH (bech32):     " + p2wpkh);
            Console.WriteLine("P2SH-P2WPKH (seg-p2sh): " + p2sh_p2wpkh);
            Console.WriteLine();

            // xprv/xpub 示例（accountKey 层）
            Console.WriteLine("Extended Private Key (xprv): " + accountKey.GetWif(network));
            Console.WriteLine("Extended Public Key  (xpub): " + accountKey.Neuter().GetWif(network));
            Console.WriteLine();
            Console.WriteLine("注意：示例默认 TestNet，真实资金请在离线/受控环境操作并妥善保存助记词与私钥。");

        }
    }
}
