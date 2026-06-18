using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class WaveDirector
{
    public class WaveModifiers
    {
        public GameObject WaveSpecialBonusEnemy { get; set; } = null;
        public float EnemyScaling { get; set; } = 1.0f;
        public float CreditGatherScaling { get; set; } = 1.0f;
        public float InitialAmbush { get; set; } = 0.0f;
        public int BonusSkullWaves { get; set; } = 0;
        public Dictionary<Type, int> BonusSkullSwarm { get; set; } = new();
        public void Reset()
        {
            WaveSpecialBonusEnemy = null;
            EnemyScaling = CreditGatherScaling = 1;
            InitialAmbush = 0.0f;
            BonusSkullWaves = 0;
            BonusSkullSwarm.Clear();
        }
        public void CloneValues(WaveModifiers other)
        {
            WaveSpecialBonusEnemy = other.WaveSpecialBonusEnemy;
            EnemyScaling = other.EnemyScaling;
            CreditGatherScaling = other.CreditGatherScaling;
            InitialAmbush = other.InitialAmbush;
            BonusSkullWaves = other.BonusSkullWaves;
            BonusSkullSwarm = new(other.BonusSkullSwarm);
        }
    }
    public static List<GameObject> PossibleEnemies()
    {
        List<GameObject> result = new(EnemyPool);
        if (TemporaryModifiers.WaveSpecialBonusEnemy != null && result.Count <= 0)
            result.Add(TemporaryModifiers.WaveSpecialBonusEnemy);
        return result;
    }
    public static List<WaveCard> AssociatedWaveCards;
    public static readonly List<GameObject> EnemyPool = new();
    public static readonly WaveModifiers PermanentModifiers = new();
    public static readonly WaveModifiers TemporaryModifiers = new(); //Permanent modifiers, but with bonuses applied after cloning values
    private static int CurrentAssociatedWaveCardNumber = 0;
    public static float WaveProgressPercent { get; set; } = 0f;
    public static void Update()
    {
        if (!Main.WavesUnleashed)
            return;
    }
    public const int MaxCards = 5;
    public static float Credits = 0, CreditsSpent = 0;
    public static float PlayRecoil { get; private set; }
    public static readonly List<WaveCard> Deck = new();
    public static readonly List<WaveCard> Board = new();
    public static int CardsPlayed = 0;
    public static int WaveNum = 0;
    public static int HighScoreWaveNum = 0;
    public static float WaveMult { get; private set; } = 1.0f;
    public static float EnemyScalingFactor => TemporaryModifiers.EnemyScaling;
    public static bool WaveActive { get; set; } = false;
    public static bool WaitingForCardDraw { get; set; } = false;
    public static int SkullEnemiesActive { get; set; } = 0;
    public static void Reset()
    {
        Main.PylonProgressionNumber = 0;
        Player.PickedLowerDifficultyWaveCard = false;
        CurrentAssociatedWaveCardNumber = 0;
        WaveNum = 0;
        WaveMult = 1.0f;
        CardsPlayed = 0;
        CreditsSpent = 0;
        Deck.Clear();
        Board.Clear();
        WaveProgressPercent = 0;
        EnemyPool.Clear();
        PermanentModifiers.Reset();
        TemporaryModifiers.CloneValues(PermanentModifiers);
        WaveActive = WaitingForCardDraw =false;
        SkullEnemiesActive = 0;
    }
    public static void FixedUpdate()
    {
        if ((SkullEnemiesActive > 0 || WaveActive) && WaveNum >= 15)
            AudioManager.SetMusic(AudioManager.BattleTheme, 1);
        if (!Main.WavesUnleashed)
        {
            Reset();
            return;
        }
        else if(!WaveActive)
        {
            return;
        }
        float creditsNeededToPassWave = ((25 + WaveNum * 0.25f) * (0.5f * WaveMult + 0.5f * TemporaryModifiers.CreditGatherScaling)) + TemporaryModifiers.InitialAmbush * 0.5f;
        float cardsPlayedPercent = Mathf.Min(1, CardsPlayed / 10f);
        float creditsSpentPercent = Mathf.Min(1, CreditsSpent / creditsNeededToPassWave);
        float percentWaveComplete = cardsPlayedPercent * 0.2f + 0.8f * cardsPlayedPercent * creditsSpentPercent;
        if (Main.DebugSettings.SkipWaves)
            percentWaveComplete = 2;
        WaveProgressPercent = Mathf.Min(1, percentWaveComplete);
        bool waveComplete = percentWaveComplete >= 1;
        if (!waveComplete)
        {
            DrawNewCards();
            UpdateMulligans();
            if (Utils.RandBool(1000)) //roughly 10 seconds to mulligan a random card
                MulliganRandomCard();
            if (PlayRecoil <= 0)
                TryPlayingAssociatedWaveCards(WaveProgressPercent);
            if (PlayRecoil <= 0)
                TryPlayingCard();
            GatherCredits();
        }
        else if(Board.Count <= 0 || Main.DebugSettings.SkipWaves) //All played cards have finish processing
        {
            EndWave();
        }
        ProcessPlayedCards();
        if (PlayRecoil > 0)
            PlayRecoil -= Time.fixedDeltaTime * WaveMult;
    }
    public static void StartWave()
    {
        ++WaveNum;
        if (WaveNum > HighScoreWaveNum)
        {
            HighScoreWaveNum = WaveNum;
            PlayerData.SaveInt("HighscoreWave", HighScoreWaveNum);
        }
        foreach(Player p in Player.AllPlayers)
            p.OnWaveStart(WaveNum);
        WaveActive = true;
        CardManager.ApplyChosenCard(); //Applies permanent modifiers, updates temp modifiers, spawns loot
        CardsPlayed = 0;
        Credits = TemporaryModifiers.InitialAmbush;
        Credits += WaveNum;
        CreditsSpent = 0;
        if(WaveNum != 1)
            MulliganAllCards();
        CurrentAssociatedWaveCardNumber = SkullEnemiesActive = 0;
        UnlockCondition.CheckAllUnlocksForCompletion();

        if (Main.CurrentPylon != null)
            Main.CurrentPylon.AddQuests();
    }
    public static void EndWave()
    {
        foreach (Player p in Player.AllPlayers)
            p.OnWaveEnd(WaveNum);
        WaveActive = false;
        WaveMult += 0.1f;
        if(WaveNum >= 15)
        {
            if(Player.Instance.Body is ThoughtBubble && !Player.HasAttacked)
                UnlockCondition.Get<ThoughtBubbleWave15NoAttack>().SetComplete();
            if(Player.Instance.Body is Gachapon)
            {
                if(!Player.PickedLowerDifficultyWaveCard)
                    UnlockCondition.Get<GachaponWave15AllSkullWaves>().SetComplete();
                if(Player.Instance.BestPowerCountIncludingStacks <= 21)
                    UnlockCondition.Get<GachaponBlackjack>().SetComplete();

            }
            if (Player.Instance.Body is Bubblemancer && !Player.HasBeenHit)
                UnlockCondition.Get<BubblemancerPerfection>().SetComplete();
        }
        if(WaveNum >= 15)
            UnlockCondition.Get<ThoughtBubbleUnlock>().SetComplete();
        if(WaveNum >= 30 && Player.Instance.Body is Gachapon)
            UnlockCondition.Get<GachaponAddicted>().SetComplete();
        if(TemporaryModifiers.WaveSpecialBonusEnemy == EnemyID.Gatligator || PermanentModifiers.WaveSpecialBonusEnemy == EnemyID.Gatligator)
            UnlockCondition.Get<FizzyUnlock>().SetComplete();
        if (Player.Instance.Body is ThoughtBubble && 
            (TemporaryModifiers.WaveSpecialBonusEnemy == EnemyID.Infector ||
            PermanentModifiers.WaveSpecialBonusEnemy == EnemyID.Infector))
            UnlockCondition.Get<ThoughtBubbleIndistinguishable>().SetComplete();
        CardManager.ResolveChosenCard(); //Gives loot and resolves cards, also sets the current card to -1
        CurrentAssociatedWaveCardNumber = 0;
        WaitingForCardDraw = true;
        UnlockCondition.CheckAllUnlocksForCompletion();

        Main.CurrentPylon.IncrementWave();
    }
    public static void GatherCredits()
    {
        Credits += 0.75f * TemporaryModifiers.CreditGatherScaling * Time.fixedDeltaTime;
    }
    public static void DrawNewCards()
    {
        while (Deck.Count < MaxCards)
        {
            var v = WaveDeck.DrawCard();
            v.MulliganDelay *= 0.5f;
            Deck.Add(v);
        }
    }
    public static void UpdateMulligans()
    {
        float ambushMult = 1f;
        if(TemporaryModifiers.InitialAmbush > CreditsSpent)
        {
            float bonusSpeed = (TemporaryModifiers.InitialAmbush - CreditsSpent) / 100f;
            ambushMult += bonusSpeed;
        }
        foreach(WaveCard card in Deck)
        {
            card.MulliganDelay -= Time.fixedDeltaTime * ambushMult * TemporaryModifiers.CreditGatherScaling;
        }
    }
    public static void MulliganRandomCard()
    {
        int rand = Utils.RandInt(Deck.Count);
        if (Deck[rand].Cost > 0 && Deck[rand].MulliganDelay <= 0)
            Deck[rand] = WaveDeck.DrawCard();
    }
    public static void MulliganAllCards()
    {
        for (int i = 0; i < Deck.Count; ++i)
        {
            Deck[i] = WaveDeck.DrawCard();
            Deck[i].MulliganDelay *= 0.5f;
        }
    }
    public static void TryPlayingCard()
    {
        for(int i = Deck.Count - 1; i >= 0; --i)
        {
            WaveCard card = Deck[i];
            bool canPlay = card.MulliganDelay <= 0;
            bool canAfford = card.Cost <= Credits;
            if(canPlay && canAfford)
            {
                PlayCard(card);
                Deck[i] = WaveDeck.DrawCard();
                return;
            }
        }
    }
    public static void TryPlayingAssociatedWaveCards(float waveProgress)
    {
        if(AssociatedWaveCards != null && AssociatedWaveCards.Count > 0 && CurrentAssociatedWaveCardNumber < AssociatedWaveCards.Count)
        {
            float interval = (CurrentAssociatedWaveCardNumber + 1f) / (AssociatedWaveCards.Count + 1f);
            if(waveProgress > interval)
            {
                var card = AssociatedWaveCards[CurrentAssociatedWaveCardNumber++];
                PlayCard(card, false, 1.25f);
                Debug.Log($"Played Special Wave Card At: [{interval * 100:##}%], {card.Patterns[0].EnemyPrefabs[0].GetComponent<Enemy>().Name().WithColor("#FF0000")}"); 
                OnSkullWavePlayed();
            }
        }
    }
    public static void OnSkullWavePlayed()
    {
        int max = Player.Instance.BlackMarketDelivery;
        if (Utils.RollWithLuck(0.1f * max))
        {
            Player.Instance.RemovePower(PowerUp.Get<BlackMarketDelivery>().Type);
            var chest = CoinManager.SpawnChest(Reward.RewardPositionChest, 3);
            --Player.Instance.BlackMarketDelivery;
        }
    }
    public static void PlayCard(WaveCard card, bool incrementCardsPlayed = true, float overridePlayDelay = -1)
    {
        Board.Add(card);
        PlayRecoil += overridePlayDelay >= 0 ? overridePlayDelay : card.PlayRecoil;
        Credits -= card.Cost;
        CreditsSpent += card.Cost;
        if(incrementCardsPlayed)
        {
            CardsPlayed++;
        }
    }
    public static void ProcessPlayedCards()
    {
        for (int i = Board.Count - 1; i >= 0; --i)
        {
            WaveCard card = Board[i];
            card.Resolve();
            if(card.Resolved)
            {
                Board.RemoveAt(i);
            }
        }
    }
}
