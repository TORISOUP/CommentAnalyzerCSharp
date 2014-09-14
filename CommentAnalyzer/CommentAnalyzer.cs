using CSharp.Japanese.Kanaxs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/**************************************************************************
 * 
 * CommentAnalyzer C# 1.0
 * Copyright (c) 2014 torisoup <tori.birdstrike@gmail.com>
 * 
 * Released under the MIT License.
 * http://www.opensource.org/licenses/mit-license.php
 * 
 * 
 * このクラスは、以下のソースを参考にして作成しました。
 * analyze.js beta 31
 * http://nicomment.batch-re-search.net/
 * Copyright (c) 2012-2013 batch-re-search.net
 * 
 *****************************************************************************/

namespace CommentAnalyzerCSharp
{
    public class CommentAnalyzeInfo
    {

        public bool isWWW { get; set; }
        public CommentAnalyzer.CommentType type { get; set; }
        public string word { get; set; }
        public CommentAnalyzeInfo()
        {
            isWWW = false;
            type = CommentAnalyzer.CommentType.unknown;
            word = null;
        }
    }

    public class CommentAnalyzer
    {
        public enum CommentType
        {
            unknown = 0,
            url,
            c2c,
            bougen,
            greeting,
            salute,
            arashi,
            lol,
            aaah,
            hihan,
            tsukkomi,
            meirei,
            question,
            request,
            cry,
            good,
            wktk,
            response,
            excite,
            shokunin,
            great
        }
        public class Distributes
        {

            public CommentType type { get; set; }
            public string word { get; set; }
            public Distributes(CommentType type, string word)
            {
                this.type = type;
                this.word = word;
            }
        }

        string exA = "[ー~アァ]";
        string exE = "[ー~エェ]";
        Regex reWWW;
        Regex reAA;
        Regex reAnti;
        Regex reTsukkomi;
        Regex reNotGood;
        Regex reGreat;
        Regex reNotBougen;
        Regex reNotGreat;
        Regex reGreeting;
        Regex reExcite;
        Regex reMoreCry;
        Dictionary<CommentType, Regex> reFilter;

        public CommentAnalyzer()
        {
            reWWW = new Regex("([^a-z]w+(\\s|$)|\\(爆?笑\\)?$|w{4,}|w{2,}$|^[^a-vx-z]*w[^a-vx-z]*$)");
            reAA = new Regex("\\n");
            reAnti = new Regex("(オ(前|[メマ][ェエイ])ラ|外野|派|[赤青]文?字|黄色|ピンク|(叩イ|言ッ)テル(ヤツ|奴)|ッテ(イウ)?(奴|ヤツ)|嫉妬|信者|擁護|アンチ|厨|動画勢|ガキ|ニワカ)");
            reTsukkomi = new Regex("((?:(?!ナンデ|カラ|イッタイ|一体).)+(スギ|ナ[イン]|ネ" + exE + "+)(ダ[ロヨ]?)|スギ|[^細デリ]カ[ヨイ]|フザケ[ルン]ナヨ?($|[^ッ])|ス[ルン]ナシ?)$");
            reGreat = new Regex("(^!+\\?+$|sugoi|suge[e\\-ー]|巧サ|(俺|僕|自分|私)ノ.*ト(違|チガ)ウ|(神|ネ申)(調教|ス[ギグ]ル)|^マジカヨ?!?$|調教(神|ネ申)|(凄|スゴ)([ス過][ギグ]|カッタ|ー*[イィ])(!*$|[^ノコタ一増事])|(凄|(^|[^探])スッ*ゲ)" + exE + "*($|[^ノコタ一増事])|パネ" + exE + "|ウメ" + exE + "|スゴ[オー!]*$|圧倒サレ[タル]|魂実装|^ス[ゥッ]?ゴイ$)");
            reNotGood = new Regex("(ド[ウー]|ナン|何|ドッチ)デモイ[イー]|(別|ベツ)ニ|ソレデ|[イ良]イ(ニ決|ジャナイ|ンジャ|ワケ|ソウ|ラシイ|トイウ|ッテ|ン?ダロ)|モウ[イ良]イ|都合ノ?|イイウナ!*$|イッパイイ|イイラスト|イイモン|[良イ]イ([エヤ]､|ヤツ|奴|ヒト|人|子|女|オンナ|男|オトコ|武|道|訳|ワケ)|(ダカラ|ダケデ|[ナ無]ク(ナッ)?テ|(シ|[アヤ]ッ)テモ)(良|イ[イー])|モ[良イ]イ(所|トコロ)");
            reNotBougen = new Regex("((マッタ|全)クソ|ヤクソウ|ジャクソン|ヤケクソ|クソ(ング|ード)|ダークソ)");
            reNotGreat = new Regex("(ス[ゴゲ][イー][ム難死]|思ッテ|(凄|ス[ゲゴ])(ク[^テナネ良イ]|ミ|イノカ?\\?)|叩[カク]|荒レ|悪|[欲ホ]シイ|レバ神調教ナノカ?\\?|(凄|スゴ)イカ(コレ)…?\\?|((何|ナニ)ガ|naniga|ソンナニ|ソコマデ|今|^)(凄|ス[ゲゴ]).*(アッタ|[イノンダ]+)カ?([分ワ]カ|(思|オモ)ワ|\\?))");
            reGreeting = new Regex("オ(疲|ツカ)レ");
            reExcite = new Regex("!");
            reMoreCry = new Regex("(ア[レ]+[\\?、…]|ナン[カダデ]|自分|ナゼ|何故|モウ泣|泣イテモ[良イ]イ|オカシイ|感動シタ)");

            reFilter = new Dictionary<CommentType, Regex>();
            reFilter[CommentType.url] = new Regex("(ttps?://|\\w{3,}\\.(com|net|jp)|[sn]m\\d+($|[^a-z\\(]))");
            reFilter[CommentType.c2c] = new Regex("((信者|アンチ|厨|ガキ)ハ.*(レ|シロ|テロ)ヨ?|コメ(ント)?非表示推奨|赤字コメ|ガキ臭イ|ブラウザ[ト閉]ジロ|コメントシカ|コメ(ント)?ス[ルン]ナ|ナラ[ミ見][ンル]ナ$|(指示|説教)厨|音量注意|ケチ[付ツ]ケテ?ル|^コメガ|^[↑↓←→]($|[^\u2196-\u2199↑↓←→])|ナゼ([ト止]メ|飛バシ?)タ|オ(前|[メマ][ェエイ])(ガナ|[ラ等])|屋上|(叩イ|言ッ|シ)テル(奴|ヤツ)ラ?|ッテ(イウ)?(奴|ヤツ)ハ|文句言ウ奴|(名人|視聴者|評論家)様|弾幕|^(歌詞)?(職人|字幕)|(アンチ|信者)乙|ネタニマジレス|他ノ動画[見ヲノ])");
            reFilter[CommentType.bougen] = new Regex("(([^バタ]|^)カス(共|ドモ)?ガ?!*$|低脳|黙レks|ビチグソ|クタバレ|(潰|ツブ)レロ|(タヒ|[シ死市氏])(ネバイイノニ|ンダ(ホウ|方)ガ)|コンナ(奴|ヤツ).*バイイノニ|(滅ベ|消エテクレレ)バイイノニ|厨房?ハシネ|(頭|アタマ)[ガノ]?(オカシイ|(ワリ[イィー]|(悪|ワル)(イ|[ス過][ギグ])))|ツマンネ" + exE + "ンダヨ|テンジャネ" + exE + "ヨ|異常者|(キチ|基地)(ガイ|外|害)|(^|ハヨ|サッサト|(ハヤ|早)ク)シネヨ?$|(^|[^a-z])k[sz]($|[^a-z])|^クズ($|[^レ])|((^|[^親ュ])バカ|馬鹿|アホ|阿呆|ボケ)(ドモ|共|ミタイ|カ|ダナ)?$|(バカ|馬鹿)(ジャネ" + exE + "ノ|(丸|マル)[ダ出]シ)|ゴミ($|[^プバノ箱袋捨])|人間ノクズ|クズ(人間|共|ドモ|ガ|ダ|シカ|達|$)|(^|[^星])屑|駄作($|(?:(?!ジャナイ).)+)|イメージ下|イラツク|(タヒ|[市死氏])(ンデ[クコ]|ネ[^ル])|(下手|ヘタ)?(クソ|糞)(ー*$|(?:(?![ワバ][ラロ]|ウ[｡､…・!]|ー|~|笑|[吹噴フ]イ|面倒|メンド|ミタイニナル|カッ(コ|ケ" + exE + ")|難|ムズ|田|コジャ|カワ[イエ]|早|速|ハヤイ|強|ツ(ヨ|" + exE + ")|ヤ(バイ|ベ" + exE + ")|[ッ､]*[泣涙]|長|ナガ|面白|ミソ|オモシ?[ロレ]|箱|ゲー).)+)|池沼|フザケ[ルン]ナヨ?|ウザイ|ウゼ" + exE + "|([キ消]エ|[ウ失]セ)ロ[ヨ!\\^]*$)");
            reFilter[CommentType.greeting] = new Regex("(^((u|ウッ?)(po?|ポ)([ー~乙]|オ?ッ?ツ|ts?u)|ウ(ポチ|プツ)(デス[ー~]*)?$|オッ?ツ|ots?u|乙|akeome|オ(久|ヒサ)シ[振ブ]リ|オヒサ[ー~]+|アケオメ|ゴキゲンヨウ|オハヨ[ウー~]|^ンc$|ノシ|^初見$)|ウポツ[ー~]*$)");
            reFilter[CommentType.salute] = new Regex("^(ノ+)$");
            reFilter[CommentType.arashi] = new Regex("(vipカラ|[@\\/](\\d*倍速|反転|逆再生|ピザ|baisoku)|^[#♯]ループ解除)");
            reFilter[CommentType.lol] = new Regex("(ジワジワ|腹筋|[吹噴](ク|イタ)|^フイタ$|(笑|ワ[ラロ])(イス|[タチッシス])|(^|[^ル])バ[ロカ]ス)");
            reFilter[CommentType.aaah] = new Regex("([アァa]{3,}|[イィ]{3,}|[ウゥ]{3,}|[エェe]{3,}|[オォo]{3,}|[ー]{3,})");
            reFilter[CommentType.hihan] = new Regex("((^|[^本])[キ気]((ショ|[モ持])(チ(ワル|悪))?(ー|イ($|[^タ])|クナッ|[ス過][ギグ])|メ" + exE + "[ヨナ]?)|器ガ?小サイ|見苦シ|犯罪者|資格ナイ|何コノ人|(グダグダ|gdgd)ダナ?$|失礼ダ|ヒデ" + exE + "ナ$|モウイイ(ヨ|カラ)?(オ(前|マエ))?…?$|イマイチ|萎エルワ$|(^|ハ)(ナイワ|ネ" + exE + "ヨ)$|ナゲ" + exE + "ヨ|失格ダ[ロヨ]|邪魔($|[^シスニ])|ウ[ルッ](サイ|セ" + exE + ")|見損ナ|迷惑($|(?:(?!ヨウニ).)+)$|([^トバ]|^)(最[悪低]|サイアク|サイテ[イー])($|ナ|[ヤダ]ナ?|[ス過][ギグ]ル)|(ツマ|クダ)[ンラ](ナ(サスギ)?$|ン|ナカッ|(ネ" + exE + "*|ナイ)($|(?:(?!ナラ).)+))|下品$|期待((外|ハズ)レ|シテイ?[タル]ノニ|シ[ス過]ギタ)|^結局.*カヨ$|虐待|台無シ|ガ残念($|[^ナ])|残念(ダ|カナ" + exA + "*|デス)?$|残念ナガラ.*(方|ホウ)ガ|(オモシ?ロ|面白)クハ?([ナ無]イ|ネ" + exE + ")($|(?:(?!ナラ).)+)|コ(レ|ンナ(ノ|動画))(ガ|[ミ見]テ|作ッ).*(オモシロ|面白)イト(オモ|思)(ウ(奴|ヤツ)|ッテ[ルン])|(ワラ|笑)エナイ|悪質|不愉?快|不謹慎$|コノ程度.+叩|[聴聞]キ(ニクイ|[ヅズ]ライ)|音質ガ?悪|(微妙|ビミョ[ウー])(ダト思|(カナ" + exA + "*|ダナ?)?$)|イイ加減ダ?ナ?$|(ナニ|何)ガ((タノ|楽)シイ|[良イ]イ|(面白|オモシ?ロ)イ|(凄|スゴ)イ)(カ[^ナ]|[ンノ])(?:(?!ノガ).)*$|(気分|性格|印象|段取リ|テンポ|マナー)ガ?(悪|ワル)|[ガモ]悪イ$|気分ノ[良イ]イ.+デハナイ|(酷|ヒド)([ス過][ギグ](ル[ワナヨ]?)?(ダロ)?|イナ)$|^酷イ(?:(?!(事件|仕打|不意)).)+|^ヒ(ドイ|デ" + exE + ")$|ショボ(スギ|イ)?$|[イ要]ラ(ネ" + exE + "*ヨ?|ナイ(ナ|デス)?$|ンナ?)$|能書キハ?[要イ]ラネ" + exE + "|賛同デキ(ン|ナイ)|(シラ|白)ケルワ?$|[合ア]ッテナイ|(何|ナ[ニン])カ(違|チガ)ウ[ナ…]*$|嫌イ(ダ?ナ)?$|ガマシイ|イイ気.+ナイ$|考エハオカシイ|秋田|(^|[^サ])[ア飽]キ(テキ)?タ([^アァー~ナノラグン]|$)|トシテドウナノ|(トシテハ?|ハ)(駄目|ダメ)(?:(?!ナノ|ト言|\\?).)*$|(痛|イタ|コダワリ)[ス過]ギ|関係(ナイ|ネ" + exE + ")ダロ|(^|[^ニ])嫉妬($|ジャン|ダロ|乙)|大概ニシロ|(カワイ|可愛|[良ヨイ])クナイ($|[^\\?])|(愚|オロ)カ(シイ)?$|ムカツク|ガッカリダ?$|シツコイ[ヨワ]?$|ベキ(コト|事)ガアルダロ|(頭|アタマ|耳|ミミ)ガ?オカシイ|(何|ナニ)考エテル?ンダ|([^機]嫌|イヤ|駄目|ダメ|余計|不自然)ダナ" + exA + "*$|(イカガ|如何)ナモ[ノン]カ|ドウカト思ウ|犯罪デスヨ$|ダケジャン$|吐キ?気ガスル|ヨウニシカ([聞キ]コエナイ|[見ミ]エナイ)|通報スル|ガイイノニ|(面白|オモシロ)サガ.*[分ワ]カラ(ン|ナイ)$|共感デキ(マセン|ナ(イ|カッタ))$|納得イカナイ$|レベル([下サ]ガリ|低スギ)|ク[オォ]リティー?低|コレガ([1一]位|優勝)(ハ|トカ)[ナ無]イ(ワー?)?($|[｡､])|パクッタノ|(何|ナニ)カ[足タ]リナイ|(オモンナイ|アリキタリ|思ワナイデクダサイ)$|面白ミ[ニガハ]|[引ヒ]クワ?$|ノガ面倒$|動キガ単調|ガ惜シイ$|(楽|タノ)シメナイ($|[^ン])|ガ下手ナノ)");
            reFilter[CommentType.tsukkomi] = new Regex("(ウワ[アァ]|オ(前|マエ)(ガ([言イ]ウナ|.+ルカ$)|ダヨ?$)|ズ[コゴ][ー~]+$|(許|ユル)シテヤレ|爆(ゼ|発シ)ロ|モウヤメテ|ヤメテ(ー|><)|ヤメタゲテ|オマワリサン|オサワリマン|^(イヤ)+$|^(オイ)+!*$|^(オイ､?(ヤメロ|[待マ]テ)?|(チョット)?([待マ]テ)+|^[待マ]テヨ?コラ$|^(オイ|バカ|ヤメロ)+$|(チョ|オマ|エ)ッ?|^エ\\?*$|ン|オ(､|…+)オウ|ウ(､|…+)ウン|セ(､|…+)セヤナ|嘘乙)$|審議|(マタ|ヤッパリ)(アナタ|オ(マエ|前))カ|コッチ(ミ|見)ンナ|仕事シロ|(分|ワ)カルダロ$|日本語デ|ジャネ" + exE + "カ|^コレハヒドイ$)");
            reFilter[CommentType.meirei] = new Regex("(トヤカク.+ナヨ?$|ナラ.+[ロレケエ]ヨ$|[落オ]チ[着ツケ]{2,}|シナサイ|説明書(嫁|[読ヨ]メ)|[^ナ][イ行叩]ケ|([^レ]ヤ|ダマ|黙)レ|([^ノハロ]エ|[シセ見ミゲメ])ロ|[ヤス]ンナ|[ンル]ナヨ$|ヤメテ(クレ)?)($|[ヨヤ])");
            reFilter[CommentType.question] = new Regex("(^(ナゼ|何故)([^カ]|$)|^ナン(デ|ダ($|[^カ]))|^ドウシテ|ナンデダロウ?ネ?$|[^!]\\?|ドウ.*ツモリダ$|^\\?$|(コト|事|大丈夫)(ナノ)?カナ" + exA + "*$)");
            reFilter[CommentType.request] = new Regex("(音[量声]?((大キ|オオキ|デカ)(イ|スギ)|(チ[イィッ]|小)((サ|チャ)(イ|スギ)|セ" + exE + "))|(スルー|[ノタ](方|ホウ))ガ((ナン|何)トナク)*((見易|[良イ])(イ|カッタ))|オ(願|ネガ)イシマス|作ッテ[ー~]$|([聞聴キ](キ|イテミ)|見)(タ(イ|カッタ)|テ(" + exE + "+|ミタイ))(ネ" + exE + "*|ナ" + exA + "*)?$|(希望|(望|頼|タノ)ム)$|^モウ.*ハイイヨ|^ハヨ|[出シセキレッ]テ[ホ欲]シ(カッ|イ($|[^カンノト時]))|[欲ホ]シ(イ|カッタ)(デス)?[ナネワヨ]*" + exA + "*[…、]?|[ホ欲]シス$|マダー|(クレヨ?|[^オ]ハヨ|[デャバテ]イイノニ|((早|ハヤ)ク).*ンダ|スベキ)$|ナラ.+ベキ|デモ[良イ]イヨウナ|不要ダッタカモ|モウ(少シ|(チョ(イ|ット))).*((方|ホウ)ガ|ト)イイ|クレルト[良イ]イ|((モット|(チョ(イ|ット))).*[イタ]|[シデ出コ来レ](ナイ|ネ" + exE + ")|マダ|イラナイ)カナ" + exA + "*$|(チョ(ット|イ).*(ク[見ミ]エ|スギカ)))");
            reFilter[CommentType.cry] = new Regex("(涙($|[^目])|涙目デ|涙目ニナッタ$|泣|ナケルー|^ナミダ[出デガ]|ホロリト|ナミダガ[ト止]マ|ノд`|画面ガボヤケ|(セツ|切)ナ(イ|[過ス]ギ)|胸ニコミ[上ア]ゲル|グット[来キク][タル]|ナケ(ル|テキタ)$|感動|^[^;]*;[ωエェ_]?;[^;]*$|ナキ(ソウ|マシタ)|ナイ(チャウ|テシマ)|^ナ(イタ|ク)$|目カラ.*ガ|セルフエコノミー|ウルッ[テト][キ来]テ?タ|グスン$|^\\(?t[ao]?t\\)?$|^ブワッ$|鼻ノ頭ガツー?ン|ナンカ目カラ|ジ[ー~]ント[来キク]|ココデナク$|心ガ浄化)");
            reFilter[CommentType.good] = new Regex("(アリガ(ト[ー~ウ]?|タ[ヤイ])|才能ノ(塊|(無駄|ムダ)(使|遣|ズカ|ヅカ)イ)|胸(熱|アツ)|ダケデモ?タマゲ[ルタ]|感心スル|^ヤル(ナ[ー~!]*|ネ" + exE + ")$|脱帽|ガ気ニ[入イ]ッタヨ?$|他ノ追随ヲ|臨場感ガ?アル|感情コモッテル|(超|神|ネ申|変態)画質|神業|見事|(凄|ス[ゴゲ])[クイー]似?合|ハイ[シ死]ンダ($|ー)|完成度ガ?(異常|高|タ[カケ])|マッチシテ(マス|ル)|[合ア](ッテル$|ウ(ネ|ナ($|" + exA + ")))|(巧|上手|(^|[^思])ウマ)(ダ|カッタ$|イ|[ス過][ギグ])($|ダロ|デス|ジャン|ヤン|ワ" + exA + "*$|([ゾネルヨナ]|ノ[ウゥガ]))|気ガ[ア合]イソウ|[ノ伸]ビ(ロ$|テ[欲ホ])|(コレハ|モット)[ノ伸]ビル|評価サレルベキ|(イヤ|癒)サレル|心ガ[温暖]|レベルガ?(異常|高([^イ]|イ($|[^ノ]))|タカ|タケ)|(ガンバ|頑張)ッタナ" + exA + "*$|タイシタモン|惚レタヨ?$|アラカワ|カワユ$|カワユス|(可愛|カワイ)サ$|(コレハ|ハイハイ)マイリスト?|[デテ]マイリスト?|マイリスト?(決定|スルシカ|セザル|余裕)|マイリスト?ニ?[入イ]|([ミ見魅]|[聞聴キ]キ)[イ入][ッル]|(所|トコロ)ガ[良ヨイ]イカモ|(^|[^男女♂♀])[好ス]キ(カモ|[ス過]ギ[ルテ]|ダ[ワナー~]*)?(コレ)?$|(ココ|(凄|ス[ゴゲ])[ーイク]|個人的ニハ?|メチャ([メク]チャ)?)[ス好]キ|(大|ダイ)[ス好]キダワ?" + exA + "*?$|オ(洒落|シャレ)|オサレ($|ダ)|(毎日|何度モ)[見来観]|kawaii|good|(カー*[ワアァ]+ー*[イィエユ]+ー*|可愛)(クッ?テ|カッタ|ラシイ|[ス過][ギグ]|[イィエェー~])|素敵|ステキ|(スン?バラ|素晴ラ?)シ|アザ(イ$|ト[イサス]|テ" + exE + ")|カッ[コケ]([良イ][イィー~]|" + exE + "+)|[聴聞]キ([取ト]リ)?ヤスイ|([^ツ]|^)[ヨ良]([ス過]ギ|サゲ)|((^|[^不])自然|(滑|ナメ)ラカ)(ダ(ナ|ネ" + exE + "*)?$|[ス過]ギ)|(自然|リアル)ナ動キ|動キガ(自然|リアル)|(マ[アァ]|メッ?チャ)(良|ヨカ)|ク[オォ]リティー?ガ?(異常|高[^低]*$|タ[カケッ]|パ[ナネ]|ヤ[バベヴ])|^(表情|モーション)?ガ?ヤ((バ|ヴァ)イ|ベ" + exE + ")!*$|メッ?チャイイ|.コノ(ク[オォ]リティー?|完成度)(デアル)?$|^ヤバイ[ナヨ]コレ$|コ[レコ](ハ|マジデ?)*ヤ(バイ|ベ" + exE + ")|コ(コ|[コッ]カラ).*ヤバイ$|コレハ売レル|(売レル|商(売(出来|デキ)ル|品|業))レベル(ダ(ッタ|[ワヨナロ])*)?$|綺麗|キレイ|深イ作品|抜群|[傑良名]作|^超?大作$|美シ|[ウフ]ツクシ|美声|天使(ダ(ッタ)?)?$|エエノ[ウゥー~]+♪*$|(楽|タノ|嬉|ウレ)シ(ク見レル|カッタ($|[^事思])|イ((限|カギ)リダ?|ナ" + exA + "|[ネィ。♪★☆ヨナゾ])*$|メル(カラ|ジャン|[ナワ]?$)|ンデルヨ)|(鳥|チキン)肌|^トリハダ$|([^バワヤャアトツ]|[^タナ]ラ|^)[イ良ヨ]([イィ]($|[^マノカケ加])|イカンジ|クナッテル|カッタ($|[^ン]))|[イ良ヨ]イネ" + exE + "*$|(?:(?!レバ|[ナタ]ラ).)*(面白|オモシ?ロ)(イ動画[^作]|カ[ッタ]|[ス過][ギグ]|イト思(?:(?!ワナ|ッテ).)+|イ[ネゾワナヨアァー~]*(!|コレ)*$)|オモシ(レ($|" + exE + "))|オメ([ー~♪]+|デト[ウー~](ゴザイマス)?)?[。!♪☆★]*$|エエ曲|名曲(ダ?ナ" + exA + "*)?$|^(マジ|コレ)?(神|ネ申)(ゲー|アニメ|動画|曲|mad|pv|bgm)|(^|[^バ風雷邪乏])(最高|完璧|ネ申|神)(!|ダワ|デシタ|デス[ワタ]?|ジャナイカ|ダナ?|[ス過][ギグ]ル?(ダロ)?)*$|ハ神$|最高[bダス]|コレハナカナカ$|ナカナカヤル|尋常ジャナイ|ハイセンス|センス(ガ[良ヨイ]イ|アル|(高|タカ)イ$|ヲ(感|カン)ジル|(アル|ヤ(バイ|ベ" + exE + "*))ナ)|シブイナ?" + exA + "*$|(サスガ|流石)(!*$|[^ノニ])|(ハジ|始)マッタナ$|凝ッテ[ルン]|人間ヲ[超コ]エタナ?$|ヨク(ヤッタ|(デキ|出来)テイ?[ルン])(ヨ?ナ" + exA + "*)?!*$|^(gj|8{3,})+[!♪]*|[^a-z]gj($|[!♪]*)|go*djob|グッジョブ|ガ完璧ダ|完璧ニ再現シ|分ガ.*アットイウ間|ズット([見ミ]|[聞キ]イ)テイ?タイ|(ホント|本当)ニ.*[ル間]ミタイ|ニコニコシチャウ|半端ナイ|天才ダ|(^|[ウヤ])プロダ[ナヨロ]?$|^プロ(イ$|ノ(本気|仕事)|トシカ)|野生ノプロ|((個人|俺|私)的ニ|コレ|ノ中デ)ハ?優勝|^優勝(ダワ?)?$|優勝(イケル|ダト思ウ)|勧メラレル.*ダナ" + exA + "*$|saiko\\-|愛ヲ(感|カン)ジ[タル]|ヌルヌル動|[魂力][ガノ]?((入ッ|[込コ]メラレ)[テト]イ?ル|[込コ]モッタ)|(何度.*テモ|テテ)[飽ア]キ|尊敬スル[ナワ]?$|洗練サレテイ?ル|刺激サレルネ" + exE + "*|再現度ガ?高イ|タカッタンダ" + exA + "|テテ(気持|キモ)チイ+ナ?$|期待ノ.*上|コレハ?タマラン[ワー~]?$|俺得|((恐|オソ)ロシイ|驚愕ノ|(ナン|何)トイウ)(ク[オォ]リティ)|振リ?込メ(バ|ナイ)|ダケデコ(コマデ|ンナニ).*ルノカ|(選曲|ナイス){2,}|^(ナゴ|和)ムナ?)");
            reFilter[CommentType.wktk] = new Regex("((^|[^a-oq-z]|[a-z]{2,})(wk|kt)|期待(?:(?!シタ(程|ホド)|シテ.*ミレバ.*カヨ|シナイナラ|シテミタラ|サレテ|シロト|ハ([持モ]テ|出来|デキ)|値|トカ|シテ?タ).)*$|サ" + exA + "ココカラ|ワク[^ダネ]|kita|(嫌|イヤ)ナ(予感|ヨカン)|(^|[^ウガテー])[来ク]ル[ゾカ][!\\?]*$|オ[待マ]チシテマ|[待マ](タレル|ッテ(イ?[タル]ヨ[ー~]*|タンダ|イ?マス|マ[シス]タ" + exA + "*))!*$|待ッテ[タル](?:(?!ノニ).)*$|^超?マッテ[タル]!*$|(^|[^生イテ])[キ来][タテ](レ[ウゥ]|タノカ!*$)|^キタ(コレ)|キタキタ($|[^杯])|^キタ" + exA + "*!*$|新作[キ来]タ|(^|[^出テ])[キ来]タ" + exA + "|楽シミ($|(?:(?!タイダケ|方).)+)|^(ザワ[・…]*)+$|[ニモ]機体$|ト[聞キ]イテ$|(続|ツヅ)キガ[気キ]ニナ|^[キ来]タ$)");
            reFilter[CommentType.response] = new Regex("(([オo]コ?k|^(ウ[ンム])+|^(デス|ソウ(デス|ダ(ヨ|ロウ)?)?)[ネヌナヨ][!アァエェー~]*|^ネ[ー~]*|^(確|タシ)カニ[ナネ]?|^[アァオォー~､]*ソウナンダー?|^(ホウ)+|^ナルホド[ー~]*([…]*ワカラン[…]*)?|^ヘ" + exE + "|デ[良イ]イ(ト思ウ)?ヨ)$)");
            reFilter[CommentType.excite] = new Regex("(^ガタッ|^([エェ]{2,}|[オォ]{2,})([ー~､。].*)?$)");
            reFilter[CommentType.shokunin] = new Regex("([\u2197\u2198\u223a\u223b\u224b\u2255\u2256\u2262\u2263\u2500\u2523\u2550\u2508\u256d\u2581-\u258f\u2593\u2595\u25c0\u25e3-\u25e5\u261c\u2661\u2663\u2666\u2667\u266b\u2732\u273b\u273c\u273e\u2740\u2741\u2744\u2745\u2764\u273f\u2799\u27aa-\u27ae\u5344]|^[\u2665\\s　]+|^[○●☆★…・゜｡=≡三:\\.\\+\\*\\s]{3,}$|[↑↓←→\u2196-\u2199]{4,})");
        }

        public Distributes DoDistributes(string txt)
        {
            if (txt.Length == 0) { return null; }
            foreach (var x in reFilter)
            {
                var m = x.Value.Match(txt);
                if (m.Success)
                {
                    return new Distributes(x.Key, m.Value);
                }
            }
            return null;
        }

        public CommentAnalyzeInfo AnalyzeComment(string text)
        {
            var analyzeInfo = new CommentAnalyzeInfo();
            var checkText = "";
            var orgCheckText = "";


            if (reAA.Match(text).Success)
            {
                //職人判定
                analyzeInfo.type = CommentType.unknown;
            }
            else
            {
                // コメントの文字種統一
                var katakana = KanaEx.ToKatakana(text);
                var hankaku = KanaEx.ToHankaku(katakana);
                var zenkaku = KanaEx.ToZenkakuKana(hankaku);
                checkText = zenkaku.ToLower();

                #region 置換
                // ｗを消す前にｗの有無判定
                if (reWWW.Match(checkText).Success) { analyzeInfo.isWWW = true; }

                // ｗの除外
                checkText = Regex.Replace(checkText, "w+($|[^k])", x => x.Groups[1].ToString());

                // 「・・・」の統一
                checkText = Regex.Replace(checkText, "・{2,}", "…");

                // 「ちくしょう」な意味の「クソ」は事前に外しておく
                checkText = Regex.Replace(checkText, @"^(クソ|糞)[ウゥオォ]*[\s!､ッ・…]", "");

                // 空白と行末の句読点を除去
                checkText = Regex.Replace(checkText, @"\s", "");
                checkText = Regex.Replace(checkText, @"[｡・…]+$", "");

                // 語尾の///を除去
                checkText = Regex.Replace(checkText, @"\/+$", "");

                // 強調カッコ除去
                checkText = Regex.Replace(checkText, @"(^\[|\]$)", "");

                // うっかりEnter除去
                if (!(new Regex("「")).Match(checkText).Success &&
                   (new Regex("」")).Match(checkText).Success)
                {
                    checkText = Regex.Replace(checkText, @"」", "");
                }

                // 台詞は対象外
                checkText = Regex.Replace(checkText, @"「.*」", "「@@@」");

                #endregion

                orgCheckText = checkText;

                #region コメントの解析
                // 逆説の接続詞がある場合はそれ以降を分析対象とする
                var matchText = new Regex("^(.+)(ダガ|ケド)(.*)$").Match(checkText);
                if (matchText.Success)
                {
                    var s2 = matchText.Groups[2].ToString();
                    var s3 = matchText.Groups[3].ToString();
                    checkText = s2 + s3;
                }

                var dist = DoDistributes(checkText);
                if (dist != null)
                {
                    analyzeInfo.type = dist.type;
                    analyzeInfo.word = dist.word;
                }
                else
                {
                    // 逆接の接続詞以降で分析できなかったら前方を使用する
                    checkText = matchText.Groups[1].ToString();
                    dist = DoDistributes(checkText);
                    if (dist != null)
                    {
                        analyzeInfo.type = dist.type;
                        analyzeInfo.word = dist.word;
                    }
                    else
                    {
                        checkText = orgCheckText;
                    }
                }
                #endregion
            }
            //--- 振り分けたコメントの特例処置
            var checkWord = "";

            // 真剣なコメントのはずなのに「ｗ」がついていたら分類不能に
            if (analyzeInfo.type == CommentType.cry ||
                analyzeInfo.type == CommentType.bougen ||
                analyzeInfo.type == CommentType.hihan)
            {
                if (analyzeInfo.isWWW)
                {
                    analyzeInfo.type = CommentType.unknown;
                }
            }

            // 「つまんねぇぇぇぇぇぇ」は絶叫から批判に
            if (analyzeInfo.type == CommentType.aaah && checkText.IsMatch("ツマ[ラン]ネ"))
            {
                analyzeInfo.type = CommentType.hihan;
            }

            // 「いい意味で」の批判・暴言は賞賛に
            if (analyzeInfo.type == CommentType.bougen || analyzeInfo.type == CommentType.hihan)
            {

                if (checkText.IsMatch("[良イ]イ(イミ|意味)デ"))
                {
                    analyzeInfo.type = CommentType.good;
                }
                else if (checkText.IsMatch("祝ッテヤル"))
                {
                    analyzeInfo.type = CommentType.good;
                }
                else if (checkText.IsMatch("液晶"))
                {
                    analyzeInfo.type = CommentType.good;
                }
                else if (checkText.IsMatch("時報"))
                {
                    analyzeInfo.type = CommentType.unknown;
                }
            }

            // ただの「....」は職人から外す
            if (analyzeInfo.type == CommentType.shokunin &&
                checkText.IsMatch("^[\\.・｡]+$"))
            {
                analyzeInfo.type = CommentType.unknown;
            }

            if (analyzeInfo.type == CommentType.question)
            {
                // 落胆の意味での「なんだ」
                if (checkText.IsMatch("^ナンダ.+カ[アァー~!]*$"))
                {
                    analyzeInfo.type = CommentType.unknown;
                }
                else if (checkText.IsMatch("((スコ|少)シ|(チョ|モ)ット).*(良イ|([^事ト])カナ[アァー~]*$)"))
                {
                    // 要望的な意味
                    analyzeInfo.type = CommentType.request;
                }
                else if (
                   reFilter[CommentType.cry].Match(checkText).Success &&
                   reMoreCry.Match(checkText).Success)
                {
                    // 「あれ？目からｘｘが」などの何で泣いてるんだ自分系は感動に
                    analyzeInfo.type = CommentType.cry;
                }
            }

            if (analyzeInfo.type == CommentType.good)
            {
                if (!analyzeInfo.isWWW && checkText.IsMatch("ノニ$"))
                {
                    // 「～のに」は批判
                    analyzeInfo.type = CommentType.hihan;
                }
                else if (!analyzeInfo.word.IsMatch("^(カッ|カワ|レ)") && reNotGood.Match(checkText).Success)
                {
                    // 「いい」は「いい」でも「どうでもいい」とか「別にいい」
                    analyzeInfo.type = CommentType.unknown;
                }
                else if (checkText.IsMatch("[^キシチニヒミリエケセテネヘメレビ](方|ホウ)ガ") &&
                    checkText.IsMatch("(歌イ方|センス|コウイウ|(コッチ|今回)ノ)"))
                {
                    if (checkText.IsMatch("(面白|オモシロ)"))
                    {
                        // 「～の方が面白い」は批判
                        analyzeInfo.type = CommentType.hihan;
                    }
                    else
                    {
                        if (analyzeInfo.word.IsMatch("^(ガウマ|上手)"))
                        {
                            // 「～の方が上手い」はその他
                            analyzeInfo.type = CommentType.unknown;
                        }
                        else
                        {
                            // 「～の方が良い」は要望(「～が良い」と繋がっている場合のみ)
                            checkWord = analyzeInfo.word;
                            if (checkWord.IndexOf('ガ') != 0) { checkWord = "ガ" + checkWord; }
                            if (checkText.IndexOf(checkWord) != -1)
                            {
                                analyzeInfo.type = CommentType.request;
                            }
                        }
                    }
                }
                else
                {
                    // 「これが面白いとかｗｗ」は批判
                    if (checkText.IndexOf(analyzeInfo.word + "トカ") != -1 &&
                       checkText.IsMatch("コレ[ガニ]"))
                    {
                        analyzeInfo.type = CommentType.hihan;
                    }
                }

            }


            #region bougen
            if (analyzeInfo.type == CommentType.bougen)
            {
                // 「クソ」は「クソ」でもｒｙ（
                if (checkText.IsMatch("クソ"))
                {
                    if (reNotBougen.Match(checkText).Success)
                    {
                        analyzeInfo.type = CommentType.unknown;
                    }
                }

                // 「糞」は「糞」でもｒｙ（
                if (analyzeInfo.word == "糞")
                {
                    if (checkText.IsMatch("脱糞"))
                    {
                        analyzeInfo.type = CommentType.unknown;
                    }
                }

                if (analyzeInfo.word.IsMatch("^ゴミ"))
                {
                    if (checkText.IsMatch("(イチゴミ|(^|[^ン])ナゴミ|粗大|燃|ダイゴミ|ゴミガ(残|ノコ))"))
                    {
                        analyzeInfo.type = CommentType.unknown;
                    }
                }

                if (analyzeInfo.word.IsMatch("カス$"))
                {
                    if (checkText.IsMatch("([^コ来]|^)イカス"))
                    {
                        analyzeInfo.type = CommentType.good;
                    }
                    else if (checkText.IsMatch("サーカス"))
                    {
                        analyzeInfo.type = CommentType.unknown;
                    }
                }

                if (analyzeInfo.word.IsMatch("バカ|頭ガ?オカシイ"))
                {
                    if (checkText.IsMatch("愛スベキ"))
                    {
                        analyzeInfo.type = CommentType.good;
                    }
                    else if (checkText.IsMatch("オイヤメロ"))
                    {
                        analyzeInfo.type = CommentType.tsukkomi;
                    }
                    else if (checkText.IsMatch("(俺|オレ|僕|ボク|自分|私)[ハガモ]"))
                    {
                        analyzeInfo.type = CommentType.unknown;
                    }
                }

            }
            #endregion

            #region hihan

            if (analyzeInfo.type == CommentType.hihan)
            {
                if (analyzeInfo.word == "関係ナイダロ" && checkText.IsMatch("(ナイダロ!|千早|イイ(加減|カゲン)ニシロ)"))
                {
                    // テンプレ
                    analyzeInfo.type = CommentType.tsukkomi;
                }
                else if (analyzeInfo.word.IsMatch("キモイイ") && checkText.IsMatch("動キモイイ"))
                {
                    analyzeInfo.type = CommentType.good;
                }
                else if (checkText.IsMatch("ナラ(ヨ|良)カッタ"))
                {
                    analyzeInfo.type = CommentType.unknown;
                }
                else if (analyzeInfo.word.IsMatch("残念ダ") && orgCheckText.IsMatch("残念ダケド"))
                {
                    analyzeInfo.type = CommentType.unknown;
                }
                else if (analyzeInfo.word.IsMatch("ヨウニシカ") && checkText.IsMatch("(本当|ホントウ?)ニ"))
                {
                    analyzeInfo.type = CommentType.good;
                }
                else
                {
                    // 「これがないと～」は賞賛(「これがないと～」と繋がっている場合のみ)
                    checkWord = "コレガナイト" + analyzeInfo.word;
                    if (checkText.IndexOf(checkWord) != -1)
                    {
                        analyzeInfo.type = CommentType.good;
                    }
                }
            }

            #endregion

            #region c2c

            if (analyzeInfo.type == CommentType.meirei || analyzeInfo.type == CommentType.request)
            {
                // 要望・命令形で視聴者向けの単語が入っていたらコメントに対するコメント扱い
                if (reAnti.Match(checkText).Success)
                {
                    analyzeInfo.type = CommentType.c2c;
                }
                else if (orgCheckText.IsMatch("^[↑↓←→]"))
                {
                    analyzeInfo.type = CommentType.c2c;
                }
                else if ((analyzeInfo.type == CommentType.request || !analyzeInfo.isWWW) && checkText.IsMatch("((誰|ダレ)カ|(テオ|ヒネ)クレ)"))
                {
                    // 「誰か」に対する要望、および「ておくれ」はその他
                    analyzeInfo.type = CommentType.unknown;
                }
            }

            #endregion

            #region meirei
            if (analyzeInfo.type == CommentType.meirei)
            {
                if (analyzeInfo.word == "シロ" && checkText.IsMatch("オモシロ$"))
                {
                    // 「しろ」は「しろ」でも「おもしろ」
                    analyzeInfo.type = CommentType.good;
                }
                else if (analyzeInfo.isWWW)
                {
                    // 命令形で「ｗ」がついていたらツッコミ扱い
                    analyzeInfo.type = CommentType.tsukkomi;
                }
                else if (analyzeInfo.word == "シロ" && checkText.IsMatch("(オモ|ム)シロ"))
                {
                    // 「しろ」は「しろ」でも「むしろ」(末尾以外の「おもしろ」も)
                    analyzeInfo.type = CommentType.unknown;
                }
                else if (analyzeInfo.word == "メロ" && checkText.IsMatch("メロメロ"))
                {
                    // 「めろ」は「めろ」でも「めろめろ」
                    analyzeInfo.type = CommentType.unknown;
                }
                else if (checkText.IsMatch("臆病者!"))
                {
                    // 「[命令]!臆病者!」はテンプレ
                    analyzeInfo.type = CommentType.excite;
                }
                else if (checkText.IsMatch("^([エゲ]ロ)+$"))
                {
                    analyzeInfo.type = CommentType.unknown;
                }
            }

            #endregion

            if (analyzeInfo.type == CommentType.wktk)
            {
                if (checkText.IsMatch("^[a-z0-9,.\"':]{10,}$"))
                {
                    // 偶然混じった「wktk」は除外
                    analyzeInfo.type = CommentType.unknown;
                }
                else if (checkText.IsMatch("[マ待]ッテタ[ラナ]"))
                {
                    // 「待ってたら～」は期待じゃない
                    analyzeInfo.type = CommentType.unknown;
                }
            }

            // 「俺ら」に対する命令・批判はその他
            if ((analyzeInfo.type == CommentType.meirei || analyzeInfo.type == CommentType.hihan) && checkText.IsMatch("俺ラ($|[^ノ])"))
            {
                analyzeInfo.type = CommentType.unknown;
            }

            if (analyzeInfo.type == CommentType.request)
            {
                if (analyzeInfo.word.IsMatch("タカッタ$") && checkText.IsMatch("(ズット|コウイウノ|コレ|コンナ)"))
                {
                    //「これが見たかった」は賞賛
                    analyzeInfo.type = CommentType.good;
                }
                else if (checkText.IsMatch("評価サレ|[伸ノ]ビテ"))
                {
                    // 評価の要望は投稿者宛ではない
                    analyzeInfo.type = CommentType.good;
                }
                else if (checkText.IsMatch("名曲$"))
                {
                    // 永遠に残って欲しい名曲
                    analyzeInfo.type = CommentType.good;
                }
                else if (analyzeInfo.word.IsMatch("[ホ欲]シ"))
                {
                    if (checkText.IsMatch("([死シ]ンデ|消エテ)"))
                    {
                        // 死んで欲しい
                        analyzeInfo.type = CommentType.bougen;
                    }
                    else if (checkText.IsMatch("((ケガ|汚)(シテ|サナイデ)|[ヤ止辞諦]メテ|ナイデ)") && !checkText.IsMatch("忘レナイ"))
                    {
                        // しないで欲しい系
                        analyzeInfo.type = CommentType.hihan;
                    }
                    else if (checkText.IsMatch("((構|カマ)ッテ|金ガ|昔欲|[欲ホ]シ(ケレ|イ(ノ?カヨ?[\\?!]*$|.*(アル|ダロ)|モノ|ナラ|ダケ))|言ッテ|[見ミ]ツケテ|[気キ][付ヅ]イテ|[呼ヨ]ンデ|冷メテ|[抱ダ]キ[締シ]メテ)"))
                    {
                        // よく分からない系(歌詞?)
                        analyzeInfo.type = CommentType.unknown;
                    }
                }
            }

            // 絶叫もしくは分類不能の驚愕系判定
            if ((analyzeInfo.type == CommentType.unknown || analyzeInfo.type == CommentType.aaah || analyzeInfo.type == CommentType.excite || analyzeInfo.type == CommentType.tsukkomi || analyzeInfo.type == CommentType.question)
                && reGreat.Match(checkText).Success)
            {
                if (checkText.IsMatch("(凄|スゴ)[ス過]ギ") || !reNotGreat.Match(checkText).Success)
                {
                    analyzeInfo.type = CommentType.great;
                }
            }

            // すごすぎて笑っている場合は驚愕
            if (analyzeInfo.type == CommentType.lol && reGreat.Match(checkText).Success && checkText.IsMatch("[過ス]ギ"))
            {
                analyzeInfo.type = CommentType.great;
            }

            // 絶叫、興奮、分類不能で「お疲れ」がついてたら挨拶
            if ((analyzeInfo.type == CommentType.unknown || analyzeInfo.type == CommentType.aaah) && reGreeting.Match(checkText).Success)
            {
                analyzeInfo.type = CommentType.greeting;
            }

            // 「すごいけど1位ではない」
            if ((analyzeInfo.type == CommentType.great || analyzeInfo.type == CommentType.good)
                && orgCheckText.IsMatch(".*([良ヨイ]イガ|ケド).*([^ラパ][ナ無]イ[感ナワヨ]?)$") && !analyzeInfo.isWWW)
            {
                analyzeInfo.type = CommentType.hihan;
            }

            if (analyzeInfo.type == CommentType.unknown)
            {
                if (checkText.IsMatch("^[wtrk]+$") && !checkText.IsMatch("^[tk]+$") && checkText.IsMatch("[wtr]") && checkText.IndexOf('k') != -1)
                {
                    // wktkのタイプミス
                    analyzeInfo.type = CommentType.wktk;
                }
                else if (checkText.IsMatch("([^ヨ]ッ|[^ッ])カナ[アァー~]*$"))
                {
                    // 分類不明の「～かな」は疑問形
                    analyzeInfo.type = CommentType.question;
                }
                else if (reExcite.Match(checkText).Success)
                {
                    // 何にも当てはまらなくて「!」がついてたら興奮にしとく
                    analyzeInfo.type = CommentType.excite;
                }
            }

            // 疑問形、分類不能で「ｗ」がついていたらとりあえず笑いに
            if ((analyzeInfo.type == CommentType.unknown || analyzeInfo.type == CommentType.question) && analyzeInfo.isWWW)
            {
                if (reTsukkomi.Match(checkText).Success && !checkText.IsMatch("(物|モ[ノン])ナンダ$"))
                {
                    analyzeInfo.type = CommentType.tsukkomi;
                }
                else
                {
                    analyzeInfo.type = CommentType.lol;
                }
            }

            return analyzeInfo;
        }
    }

    public static class ExtensionMethods
    {
        /// <summary>
        /// 対象の文字列が正規表現にマッチしているか調べる
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        public static bool IsMatch(this string str, string pattern)
        {
            return new Regex(pattern).Match(str).Success;
        }

    }
}
