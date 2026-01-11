using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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
        //if (result.Count < 0)
        //    result.Add(EnemyID.OldDuck);
        return result;
    }
    public static List<WaveCard> AssociatedWaveCards;
    public static readonly List<GameObject> EnemyPool = new();
    public static readonly WaveModifiers PermanentModifiers = new();
    public static readonly WaveModifiers TemporaryModifiers = new(); //Permanent modifiers, but with bonuses applied after cloning values
    public static float PointsSpent = 0;
    private static float PointTimer = 0;
    private static int CurrentAssociatedWaveCardNumber = 0;
    public static float WaveProgressPercent { get; set; } = 0f;
    public static int Point { get; set; }
    public static void PointUpdate()
    {
        PointTimer += Time.deltaTime;
        if (Player.Instance.IsDead)
        {
            PointTimer = 0;
        }
        if (PointTimer > 1)
        {
            Point++;
            PointTimer--;
        }
    }
    public static void Update()
    {
        if (!Main.WavesUnleashed)
            return;
    }
    public static float PityPowersSpawned = 0;
    public static float TotalPowersSpawned = 0;
    public static int MaxCards = 6;
    public static float Credits = 0, CreditsSpent = 0;
    public static float PlayRecoil;
    public static readonly List<WaveCard> Deck = new();
    public static readonly List<WaveCard> Board = new();
    public static int CardsPlayed = 0;
    public static int WaveNum = 0;
    public static int HighScoreWaveNum = 0;
    public static float WaveMult = 1.0f;
    public static float EnemyScalingFactor => TemporaryModifiers.EnemyScaling;
    public static bool WaveActive = false, WaitingForCardDraw = false;
    public static int SkullEnemiesActive { get; set; } = 0;
    public static void Reset()
    {
        Player.PickedLowerDifficultyWaveCard = false;
        CurrentAssociatedWaveCardNumber = 0;
        WaveNum = 0;
        WaveMult = 1.0f;
        CardsPlayed = 0;
        CreditsSpent = 0;
        Deck.Clear();
        Board.Clear();
        PointsSpent = PointTimer = PityPowersSpawned = TotalPowersSpawned = Point = 0;
        WaveProgressPercent = 0;
        EnemyPool.Clear();
        PermanentModifiers.Reset();
        TemporaryModifiers.CloneValues(PermanentModifiers);
        WaveActive = WaitingForCardDraw =false;
        SkullEnemiesActive = 0;
    }
    public static void FixedUpdate()
    {
        if (!Main.WavesUnleashed)
        {
            Reset();
            return;
        }
        else if(!WaveActive)
        {
            return;
        }
        PointUpdate();
        float creditsNeededToPassWave = ((40 + WaveNum * 0.5f) * (0.5f * WaveMult + 0.5f * TemporaryModifiers.CreditGatherScaling)) + TemporaryModifiers.InitialAmbush * 0.5f;
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
            if (Utils.RandInt(1000) == 0) //roughly 10 seconds to mulligan a random card
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
        Player.Instance.OnWaveStart(WaveNum);
        WaveActive = true;
        CardManager.ApplyChosenCard(); //Applies permanent modifiers, updates temp modifiers, spawns loot
        CardsPlayed = 0;
        Credits = TemporaryModifiers.InitialAmbush;
        CreditsSpent = 0;
        if(WaveNum != 1)
            MulliganAllCards();
        CurrentAssociatedWaveCardNumber = SkullEnemiesActive = 0;
        UnlockCondition.CheckAllUnlocksForCompletion();
    }
    public static void EndWave()
    {
        Player.Instance.OnWaveEnd(WaveNum);
        WaveActive = false;
        WaveMult += 0.1f;
        if(WaveNum >= 15)
        {
            if(Player.Instance.Body is ThoughtBubble && !Player.HasAttacked)
                UnlockCondition.Get<ThoughtBubbleWave15NoAttack>().SetComplete();
            if(Player.Instance.Body is Gachapon && !Player.PickedLowerDifficultyWaveCard)
                UnlockCondition.Get<GachaponWave15AllSkullWaves>().SetComplete();
            if (Player.Instance.Body is Bubblemancer && !Player.HasBeenHit)
                UnlockCondition.Get<BubblemancerPerfection>().SetComplete();
        }
        if(WaveNum >= 20)
            UnlockCondition.Get<ThoughtBubbleUnlock>().SetComplete();
        if(WaveNum >= 40 && Player.Instance.Body is Gachapon)
            UnlockCondition.Get<GachaponAddicted>().SetComplete();
        CardManager.ResolveChosenCard(); //Gives loot and resolves cards, also sets the current card to -1
        CurrentAssociatedWaveCardNumber = 0;
        WaitingForCardDraw = true;
        UnlockCondition.CheckAllUnlocksForCompletion();
    }
    public static void GatherCredits()
    {
        Credits += 1.1f * Time.fixedDeltaTime * TemporaryModifiers.CreditGatherScaling;
    }
    public static void DrawNewCards()
    {
        while (Deck.Count < MaxCards)
            Deck.Add(WaveDeck.DrawCard());
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
            card.mulliganDelay -= Time.fixedDeltaTime * ambushMult;
        }
    }
    public static void MulliganRandomCard()
    {
        int rand = Utils.RandInt(Deck.Count);
        if (Deck[rand].Cost > 0 && Deck[rand].mulliganDelay <= 0)
            Deck[rand] = WaveDeck.DrawCard();
    }
    public static void MulliganAllCards()
    {
        for(int i = 0; i < Deck.Count; ++i)
            Deck[i] = WaveDeck.DrawCard();
    }
    public static void TryPlayingCard()
    {
        for(int i = Deck.Count - 1; i >= 0; --i)
        {
            WaveCard card = Deck[i];
            bool canPlay = card.mulliganDelay <= 0;
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
                Debug.Log($"Played Special Wave Card At: [{interval * 100:##}%], {DetailedDescription.TextBoundedByColor("#FF0000",card.Patterns[0].EnemyPrefabs[0].GetComponent<Enemy>().Name())}");
            }
        }
    }
    public static void PlayCard(WaveCard card, bool incrementCardsPlayed = true, float overridePlayDelay = -1)
    {
        Board.Add(card);
        PlayRecoil += overridePlayDelay > 0 ? overridePlayDelay : card.postPlayDelay;
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
