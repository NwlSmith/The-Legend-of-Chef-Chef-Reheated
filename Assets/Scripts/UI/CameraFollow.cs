using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [HideInInspector] public Vector3 top_right_bounds;
    [HideInInspector] public Vector3 bottom_left_bounds;

    [HideInInspector] public Vector3 tR;
    [HideInInspector] public Vector3 bL;
    [HideInInspector] public Vector3 camera_dimensions = new Vector3(7f, 5f, 0f);

    public Transform target;
    [HideInInspector] public float smoothing = 5f;

    private float m_intensity;
    private float m_duration;
    private float m_decrease_amt;
    private readonly float m_time_step = .05f;
    private WaitForSeconds m_time_step_wait;
    private bool m_fade;

    private Vector3 offset;
    private Coroutine shake_co;
    private Transform main_cam_trans;

    private void Awake()
    {
        m_time_step_wait = new WaitForSeconds(m_time_step);
        main_cam_trans = transform.GetChild(0).gameObject.transform;
    }

    private void Start()
    {
        offset = transform.position - target.position;
        tR.x = top_right_bounds.x - camera_dimensions.x + offset.x;
        tR.y = top_right_bounds.y - camera_dimensions.y + offset.y;
        bL.x = bottom_left_bounds.x + camera_dimensions.x + offset.x;
        bL.y = bottom_left_bounds.y + camera_dimensions.y + offset.y;
    }

    private void FixedUpdate()
    {
        Vector3 targetCamPos = target.position + offset;
        targetCamPos.x = Mathf.Clamp(targetCamPos.x, bL.x, tR.x);
        targetCamPos.y = Mathf.Clamp(targetCamPos.y, bL.y, tR.y);

        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }


    public void CameraShake(float intensity, float duration, bool fade)
    {
        m_intensity = intensity;
        m_duration = duration;
        m_fade = fade;

        if (shake_co != null)
        {
            StopCoroutine(shake_co);
        }

        if (m_fade)
            shake_co = StartCoroutine(ShakeFade());
        else
            shake_co = StartCoroutine(ShakeConst());
    }

    private IEnumerator ShakeFade()
    {
        float cur_time = 0f;
        while (cur_time < m_duration)
        {
            cur_time += Time.fixedDeltaTime;
            m_decrease_amt = m_intensity * Time.deltaTime / m_duration;
            float new_x = transform.position.x + Random.Range(-m_intensity / 50, m_intensity / 50);
            float new_y = transform.position.y + Random.Range(-m_intensity / 50, m_intensity / 50);
            transform.position = new Vector3(new_x, new_y, transform.position.z);
            main_cam_trans.rotation = Quaternion.Euler(0f, 0f, Random.Range(-m_intensity, m_intensity));
            m_intensity -= m_decrease_amt;
            yield return new WaitForFixedUpdate();
        }
        StopShake();
    }

    private IEnumerator ShakeConst()
    {
        float cur_time = 0f;
        while (cur_time < m_duration)
        {
            cur_time += m_time_step;
            float new_x = transform.position.x + Random.Range(-m_intensity / 50, m_intensity / 50);
            float new_y = transform.position.y + Random.Range(-m_intensity / 50, m_intensity / 50);
            transform.position = new Vector3(new_x, new_y, transform.position.z);
            main_cam_trans.rotation = Quaternion.Euler(0f, 0f, Random.Range(-m_intensity, m_intensity));
            yield return m_time_step_wait;
        }
        StopShake();
    }

    public void StopShake()
    {
        main_cam_trans.rotation = Quaternion.identity;
        m_intensity = 0;
        m_duration = 0;
    }
}