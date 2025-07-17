// BossController.cs - CLASE DERIVADA PARA EL JEFE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : EnemyController
{
    public enum BossPatternType
    {
        FollowWaitFollow,
        FollowOffsetPattern,
        RangedAttackPattern,
        DefensivePattern,
        SpecialAbilityPattern,
        IdlePattern
    }

    [System.Serializable]
    public class BossPatternEntry
    {
        public string name;
        public BossPatternType patternType;
        public float duration;
        public MovementData movementDataOverride;
    }

    [SerializeField] private List<BossPatternEntry> bossPatterns = new List<BossPatternEntry>();

    private Coroutine currentBossPatternCoroutine;

    // La referencia ahora es de tipo BossScript
    protected new BossScript slimeAnimation; // Usa 'new' para ocultar el miembro base de tipo SlimeAnimation

    private void Start()
    {
        InitializeController();
        PickRandomBossPattern();
    }

    protected override void InitializeController()
    {
        base.InitializeController(); // Llama al InitializeController de EnemyController

        // Obtener la referencia al BossScript
        slimeAnimation = GetComponent<BossScript>();
        if (slimeAnimation == null)
        {
            Debug.LogError("BossController: No se encontró el componente BossScript en este GameObject.");
        }
        Debug.Log("BossController inicializado.");
    }

    protected override void Update()
    {
        // La lógica del patrón es manejada por las corrutinas.
    }

    public override void SetEnemyBehaviour(int index)
    {
        if (index < 0 || index >= bossPatterns.Count)
        {
            Debug.LogError($"Índice de patrón de jefe fuera de rango: {index}. Patrones disponibles: {bossPatterns.Count}");
            return;
        }

        if (currentBossPatternCoroutine != null)
        {
            StopCoroutine(currentBossPatternCoroutine);
        }

        BossPatternEntry selectedPattern = bossPatterns[index];
        Debug.Log($"Jefe ahora usando patrón: {selectedPattern.name} ({selectedPattern.patternType})");

        switch (selectedPattern.patternType)
        {
            case BossPatternType.FollowWaitFollow:
                currentBossPatternCoroutine = StartCoroutine(FollowWaitFollowPattern(selectedPattern.movementDataOverride, selectedPattern.duration));
                break;
            case BossPatternType.FollowOffsetPattern:
                currentBossPatternCoroutine = StartCoroutine(FollowWaitFollowPattern(selectedPattern.movementDataOverride, selectedPattern.duration));
                break;
            case BossPatternType.RangedAttackPattern:
                currentBossPatternCoroutine = StartCoroutine(RangedAttackPatternCoroutine());
                break;
            case BossPatternType.DefensivePattern:
                currentBossPatternCoroutine = StartCoroutine(DefensivePatternCoroutine());
                break;
            case BossPatternType.SpecialAbilityPattern:
                currentBossPatternCoroutine = StartCoroutine(SpecialAbilityPatternCoroutine());
                break;
            case BossPatternType.IdlePattern:
                currentBossPatternCoroutine = StartCoroutine(IdlePattern(selectedPattern.duration));
                break;
            default:
                Debug.LogWarning($"Tipo de patrón de jefe no implementado: {selectedPattern.patternType}. Usando IdlePattern por defecto.");
                currentBossPatternCoroutine = StartCoroutine(IdlePattern(2f));
                break;
        }
    }

    protected void PickRandomBossPattern()
    {
        if (bossPatterns.Count == 0)
        {
            Debug.LogWarning("No hay patrones de jefe configurados en BossController.");
            return;
        }

        int randomIndex = Random.Range(0, bossPatterns.Count);
        SetEnemyBehaviour(randomIndex);

        // Llamada a RotateSlime a través de BossScript (heredado de SlimeAnimation).
        if (slimeAnimation != null && playerTransform != null)
        {
            slimeAnimation.RotateSlime(playerTransform.position);
        }
    }

    public new void OnBehaviorCompleted()
    {
        canPerformAction = true;

        // Llamada a ResetAllActionAnimations a través de BossScript (heredado de SlimeAnimation).
        if (slimeAnimation != null)
        {
            slimeAnimation.ResetAllActionAnimations();
        }

        PickRandomBossPattern();
    }

    // --- Corrutinas de Patrones del Jefe ---

    private IEnumerator FollowWaitFollowPattern(MovementData data, float duration)
    {
        Debug.Log("Iniciando patrón: Seguir, Esperar, Seguir.");

        ExecuteFollowPlayer(data);
        if (slimeAnimation != null) slimeAnimation.PlayAttackAnimation(1);
        yield return new WaitForSeconds(1f);

        BossIdleeAnimation();
        yield return new WaitForSeconds(duration);
        slimeAnimation.YouCanDamgeMe = false;

        ExecuteFollowPlayer(data);
        if (slimeAnimation != null) slimeAnimation.PlayAttackAnimation(2);
        yield return new WaitForSeconds(1f);

        BossIdleeAnimation();
        yield return new WaitForSeconds(duration);
        slimeAnimation.YouCanDamgeMe = false;

        Debug.Log("Patrón 'Seguir, Esperar, Seguir' completado.");
        OnBehaviorCompleted();
    }

    private IEnumerator FollowOffsetPattern(MovementData data, float waitedDuration)
    {
        WaitForSeconds waitedtime = new WaitForSeconds(waitedDuration);
        Debug.Log("Jefe: Iniciando patrón de ataque cuerpo a cuerpo.");
        ExecuteFollowWithYOffset(data);
        if (slimeAnimation != null) slimeAnimation.SetWalkingAnimation(true);
        yield return new WaitForSeconds(0.5f);

        slimeAnimation.PlayAttackAnimation(1);
        yield return new WaitForSeconds(1f);

        ExecuteSineFollow(data);
        slimeAnimation.PlayAttackAnimation(2);
        yield return new WaitForSeconds(2f);

        BossIdleeAnimation();
        yield return waitedtime;
        slimeAnimation.YouCanDamgeMe = false;


        Debug.Log("Jefe: Patrón de ataque cuerpo a cuerpo completado.");
        OnBehaviorCompleted();
    }

    private IEnumerator RangedAttackPatternCoroutine()
    {
        Debug.Log("Jefe: Iniciando patrón de ataque a distancia.");
        if (slimeAnimation != null) slimeAnimation.PlayAttackAnimation(1);
        yield return new WaitForSeconds(5f);
        Debug.Log("Jefe: Patrón de ataque a distancia completado.");
        OnBehaviorCompleted();
    }

    private IEnumerator DefensivePatternCoroutine()
    {
        Debug.Log("Jefe: Iniciando patrón defensivo.");
        yield return new WaitForSeconds(3f);
        Debug.Log("Jefe: Patrón defensivo completado.");
        OnBehaviorCompleted();
    }

    private IEnumerator SpecialAbilityPatternCoroutine()
    {
        Debug.Log("Jefe: Iniciando patrón de habilidad especial.");
        if (slimeAnimation != null) slimeAnimation.PlayAttackAnimation(2);
        yield return new WaitForSeconds(7f);
        Debug.Log("Jefe: Patrón de habilidad especial completado.");
        OnBehaviorCompleted();
    }

    private IEnumerator IdlePattern(float duration)
    {
        Debug.Log($"Jefe: Entrando en patrón de inactividad por {duration} segundos.");
        enemyRb2D.linearVelocity = Vector2.zero;
        if (slimeAnimation != null) slimeAnimation.SetWalkingAnimation(false);
        yield return new WaitForSeconds(duration);
        Debug.Log("Jefe: Patrón de inactividad completado.");
        OnBehaviorCompleted();
    }

    //espera (sele puede hacer daño al jefe)
    private void BossIdleeAnimation()
    {
        slimeAnimation.YouCanDamgeMe =true;
        enemyRb2D.linearVelocity = Vector2.zero;
        if (slimeAnimation != null) slimeAnimation.SetWalkingAnimation(false);
    
    }
}