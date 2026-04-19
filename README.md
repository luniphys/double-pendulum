[![.github/workflows/ci.yml](https://github.com/luniphys/double-pendulum/actions/workflows/ci.yml/badge.svg)](https://github.com/luniphys/double-pendulum/actions/workflows/ci.yml)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

# Double Pendulum

## Mathematics

Starting we have the two equations of motion for both angles $\theta_1$, $\theta_2$:

$$
(m_1 + m_2) l_1 \ddot{\theta}_1 + m_2 l_2 \ddot{\theta}_2 \cdot \cos(\theta_1 - \theta_2) + m_2 l_2 \dot{\theta}_2^2 \cdot \sin(\theta_1 - \theta_2) + (m_1 + m_2) g \cdot \sin(\theta_1) + b \dot{\theta}_1 = 0
$$

$$
m_2 l_2 \ddot{\theta}_2 + m_2 l_1 \ddot{\theta}_1 \cdot \cos(\theta_1 - \theta_2) - m_2 l_1 \dot{\theta}_1^2 \cdot \sin(\theta_1 - \theta_2) + m_2 g \cdot \sin(\theta_2) + b \dot{\theta}_2 = 0
$$

We can describe the state of the double pendulum with a vector defined as:

```math
$$
\vec{y} = (\theta_1, \theta_2, \omega_1, \omega_2)^T
$$
```

With this state vector, we can transform the equations of motions into a 1-dimensional system:

```math
$$
\dot{\vec{y}} = \begin{pmatrix} \dot{\theta}_1 \\ \dot{\theta}_2 \\ \ddot{\theta}_1 \\ \ddot{\theta}_2 \end{pmatrix} = \begin{pmatrix} \omega_1 \\ \omega_2 \\ g_1(\theta_1, \theta_2, \omega_1, \omega_2) \\ g_2(\theta_1, \theta_2, \omega_1, \omega_2) \end{pmatrix} = \vec{f}(\theta_1, \theta_2, \omega_1, \omega_2)
$$
```

<br />

To get expressions for $(\ddot{\theta}_1, \ddot{\theta}_2)$ we need to solve the linear system:

```math
$$
\begin{pmatrix} (m_1 + m_2) l_1 & m_2 l_2 \cdot \cos(\theta_1 - \theta_2) \\ m_2 l_1 \cdot \cos(\theta_1 - \theta_2) & m_2 l_2 \end{pmatrix} \cdot \begin{pmatrix} \ddot{\theta}_1 \\ \ddot{\theta}_2 \end{pmatrix} = \begin{pmatrix} -m_2 l_2 \omega_2^2 \cdot \sin(\theta_1 - \theta_2) - (m_1 + m_2) g \cdot \sin(\theta_1) - b \omega_1 \\ m_2 l_1 \omega_1^2 \cdot \sin(\theta_1 - \theta_2) - m_2 g \cdot \sin(\theta_2) - b \omega_2 \end{pmatrix}
$$
```

```math
$$
A \cdot \begin{pmatrix} \ddot{\theta}_1 \\ \ddot{\theta}_2 \end{pmatrix} = C \iff \begin{pmatrix} \ddot{\theta}_1 \\ \ddot{\theta}_2 \end{pmatrix} = A^{-1} \cdot C
$$
```

<br />

Finally we can use the Runge-Kutta method of 4th order for numeric integration, to get solutions for the state.

$$
\vec{y}_{i+1} = \vec{y}_i + \frac{h}{6} \cdot \left(\vec{\kappa_0} + 2 \vec{\kappa_1} + 2 \vec{\kappa_2} + \vec{\kappa_3} \right)
$$

$$
\vec{\kappa}_0 = \vec{f} \left(\vec{y}_i \right), \quad \vec{\kappa}_1 = \vec{f} \left(\vec{y}_i + \frac{h}{2} \cdot \vec{\kappa}_0 \right), \quad \vec{\kappa}_2 = \vec{f} \left(\vec{y}_i + \frac{h}{2} \cdot \vec{\kappa}_1 \right), \quad \vec{\kappa}_3 = \vec{f} \left(\vec{y}_i + h \cdot \vec{\kappa}_2 \right)
$$

With $h$ being the step size between a time stamp $t_i$ and $t_{i+1}$.