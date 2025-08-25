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
        public void Reset()
        {
            WaveSpecialBonusEnemy = null;
            EnemyScaling = CreditGatherScaling = 1;
            InitialAmbush = 0.0f;
        }
        public void CloneValues(WaveModifiers other)
        {
            WaveSpecialBonusEnemy = other.WaveSpecialBonusEnemy;
            EnemyScaling = other.EnemyScaling;
            CreditGatherScaling = other.CreditGatherScaling;
            InitialAmbush = other.InitialAmbush;
        }
    }
    public static List<GameObject> PossibleEnemies()
    {
        List<GameObject> result = new(EnemyPool);
        if (TemporaryModifiers.WaveSpecialBonusEnemy != null)
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
    public static int WaveNum = 1;
    public static int HighScoreWaveNum = 1;
    public static float WaveMult = 1.0f;
    public static float EnemyScalingFactor => TemporaryModifiers.EnemyScaling;
    public static bool WaveActive = false, WaitingForCard = false;
    public static void Reset()
    {
        CurrentAssociatedWaveCardNumber = 0;
        WaveNum = 1;
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
        WaveActive = WaitingForCard = false;
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
        float cardsPlayedPercent = Mathf.Min(1, CardsPlayed / 10f);
        float creditsSpentPercent = Mathf.Min(1, CreditsSpent / ((50 + WaveNum) * WaveMult));
        float percentWaveComplete = cardsPlayedPercent * 0.2f + 0.8f * cardsPlayedPercent * creditsSpentPercent;
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
        else if(Board.Count <= 0) //All played cards have finish processing
        {
            EndWave();
            /*if(WaveNum == 2)
            {
                Deck[MaxCards - 1] = WaveDeck.DrawSingleSpawn(WaveDeck.RandomEdgeLocation(), EnemyID.OldSoap, 10, 2, 0);
                Deck[MaxCards - 2] = WaveDeck.DrawSingleSpawn(new EnemyPattern(Vector2.zero, 0.5f, 0f, EnemyID.OldDuck), 2, 5, 0).ToSwarmCircle(3, 10, 0);
                Deck[MaxCards - 3] = WaveDeck.DrawSingleSpawn(WaveDeck.RandomEdgeLocation(), EnemyID.Crow, 15, 2, 0);
            }
            if (WaveNum == 3)
            {
                Deck[MaxCards - 1] = WaveDeck.DrawSingleSpawn(WaveDeck.RandomEdgeLocation(), EnemyID.OldFlamingo, 10, 2, 0);
                //Deck[MaxCards - 2] = WaveDeck.DrawSingleSpawn(WaveDeck.RandomEdgeLocation(), EnemyID.OldFlamingo, 10, 2, 0);
            }
            if (WaveNum == 4)
            {
                Deck[MaxCards - 1] = WaveDeck.DrawSingleSpawn(WaveDeck.RandomEdgeLocation(), EnemyID.Chicken, 0, 1, 0);
                Deck[MaxCards - 2] = WaveDeck.DrawSingleSpawn(WaveDeck.RandomEdgeLocation(), EnemyID.Chicken, 0, 1, 0);
            }
            if (WaveNum % 5 == 0)
            {
                Deck[MaxCards - 3] = WaveDeck.DrawMultiSpawn(WaveDeck.RandomEdgeLocation(), 6, 7, 0, 0.75f, EnemyID.Chicken, EnemyID.Chicken, EnemyID.Chicken, EnemyID.Chicken, EnemyID.Chicken);
                if(WaveNum > 20)
                {
                    Deck[MaxCards - 3].Patterns[0].Location = Vector2.zero;
                    Deck[MaxCards - 3] = Deck[MaxCards - 2].ToSwarmCircle(3, 6, 0.5f, 0.5f);
                }
                if(WaveNum >= 10)
                {
                    Credits += WaveNum * 2;
                }
            }
            if (WaveNum == 6)
            {
                Deck[MaxCards - 1] = WaveDeck.DrawSingleSpawn(WaveDeck.RandomEdgeLocation(), EnemyID.Gatligator, 10, 5, 0);
                Deck[MaxCards - 2] = WaveDeck.DrawSingleSpawn(WaveDeck.RandomEdgeLocation(), EnemyID.Gatligator, 25, 2, 0);
            }
            if (WaveNum == 7)
            {
                Deck[MaxCards - 1] = WaveDeck.DrawSingleSpawn(WaveDeck.RandomEdgeLocation(), EnemyID.OldLeonard, 20, 5, 0);
            }
            if (WaveNum >= 10)
            {
                if(WaveNum % 3 == 0)
                    Deck[MaxCards - 1] = WaveDeck.DrawSingleSpawn(WaveDeck.RandomEdgeLocation(), EnemyID.OldLeonard, 15, 5, 0);
                if (WaveNum % 4 == 1)
                    Deck[MaxCards - 2] = WaveDeck.DrawSingleSpawn(WaveDeck.RandomEdgeLocation(), EnemyID.Gatligator, 15, 5, 0);
            }*/
        }
        ProcessPlayedCards();
        if (PlayRecoil > 0)
            PlayRecoil -= Time.fixedDeltaTime * WaveMult;
    }
    public static void StartWave()
    {
        Player.Instance.OnWaveStart(WaveNum);
        WaveActive = true;
        CardManager.ApplyChosenCard(); //Applies permanent modifiers, updates temp modifiers, spawns loot
        CardsPlayed = 0;
        Credits = TemporaryModifiers.InitialAmbush;
        CreditsSpent = 0;
        if(WaveNum != 1)
            MulliganAllCards();
        CurrentAssociatedWaveCardNumber = 0;
    }
    public static void EndWave()
    {
        Player.Instance.OnWaveEnd(WaveNum);
        WaveActive = false;
        WaveMult += 0.1f;
        if(WaveNum >= 16)
        {
            if(Player.Instance.Body is ThoughtBubble && !Player.HasAttacked)
            {
                ThoughtBubbleWave15NoAttack t = UnlockCondition.Get<ThoughtBubbleWave15NoAttack>() as ThoughtBubbleWave15NoAttack;
                t.SetComplete();
            }
        }
        CardManager.ResolveChosenCard(); //Gives loot and resolves cards, also sets the current card to -1
        ++WaveNum;
        if (WaveNum > HighScoreWaveNum)
        {
            HighScoreWaveNum = WaveNum;
            PlayerData.SaveInt("HighscoreWave", HighScoreWaveNum);
        }
        CurrentAssociatedWaveCardNumber = 0;
        WaitingForCard = true;
    }
    public static void GatherCredits()
    {
        Credits += 1.15f * Time.fixedDeltaTime * TemporaryModifiers.CreditGatherScaling;
    }
    public static void DrawNewCards()
    {
        if(Deck.Count < MaxCards - 2)
        {
            Deck.Add(WaveDeck.DrawSingleSpawn(WaveDeck.RandomPositionOnPlayerEdge(), EnemyID.OldDuck, 0, 1, 0));
            Deck.Add(WaveDeck.DrawSingleSpawn(WaveDeck.RandomPositionOnPlayerEdge(), EnemyID.OldDuck, 0, 1, 0));
            //Deck.Add(WaveDeck.DrawSingleSpawn(new EnemyPattern(Vector2.zero, 0.5f, 0f, EnemyID.OldDuck), 2, 5, 0).ToSwarmCircle(3, 10, 0));
        }
        while (Deck.Count < MaxCards)
        {
            Deck.Add(WaveDeck.DrawCard());
        }
    }
    public static void UpdateMulligans()
    {
        foreach(WaveCard card in Deck)
            card.mulliganDelay -= Time.fixedDeltaTime;
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
