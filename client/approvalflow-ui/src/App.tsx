import { useState } from 'react'
import './App.css'
import { ApprovalWorkspace } from './components/ApprovalWorkspace'
import { InvoiceSubmissionForm } from './components/InvoiceSubmissionForm'
import { WorkflowMonitor } from './components/WorkflowMonitor'
import { InvoiceLifecycleProvider } from './context/InvoiceLifecycleContext'
import { useInvoiceLifecycle } from './hooks/useInvoiceLifecycle'

type View = 'submit' | 'approval' | 'workflow'

const views: Array<{ id: View; label: string }> = [
  { id: 'submit', label: 'Submission' },
  { id: 'approval', label: 'Approvals' },
  { id: 'workflow', label: 'Workflow' },
]

function Dashboard() {
  const [activeView, setActiveView] = useState<View>('submit')
  const [logoFailed, setLogoFailed] = useState(false)
  const { error, workflow = [], approvals = [] } = useInvoiceLifecycle()

  return (
    <main className="app-shell">
      <aside className="sidebar">
        <div className="brand-lockup">
          <span className="brand-mark">
            {logoFailed ? (
              <span className="brand-fallback">ZN</span>
            ) : (
              <img
                crossOrigin="anonymous"
                src="/src/assets/zionnet-logo.png"
                alt="Zion-Net logo"
                onError={() => setLogoFailed(true)}
              />
            )}
          </span>
          <div>
            <strong>ApprovalFlow</strong>
            <span>Invoice Lifecycle Platform</span>
          </div>
        </div>

        <nav className="nav-list" aria-label="Dashboard views">
          {views.map((view) => (
            <button
              className={activeView === view.id ? 'nav-item active' : 'nav-item'}
              key={view.id}
              type="button"
              onClick={() => setActiveView(view.id)}
            >
              {view.label}
            </button>
          ))}
        </nav>
      </aside>

      <section className="content-shell">
        <header className="topbar">
          <div>
            <p className="eyebrow">Distributed Console</p>
            <h1>Invoice Lifecycle Command Center</h1>
          </div>
          <div className="kpi-strip">
            <div>
              <span>Pending</span>
              <strong>{approvals?.length ?? 0}</strong>
            </div>
            <div>
              <span>Tracked</span>
              <strong>{workflow?.length ?? 0}</strong>
            </div>
          </div>
        </header>

        {error && (
          <p className="error-banner">
            {error.status ? `${error.status}: ` : ''}
            {error.message}
          </p>
        )}

        <div className="view-stack">
          {activeView === 'submit' && <InvoiceSubmissionForm />}
          {activeView === 'approval' && <ApprovalWorkspace />}
          {activeView === 'workflow' && <WorkflowMonitor />}
        </div>
      </section>
    </main>
  )
}

function App() {
  return (
    <InvoiceLifecycleProvider>
      <Dashboard />
    </InvoiceLifecycleProvider>
  )
}

export default App
