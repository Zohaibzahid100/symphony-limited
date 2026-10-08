/* ============================================================
SYMPHONY LIMITED — GLOBAL JAVASCRIPT (STATIC VERSION)
Only interactive logic — all content is in HTML
============================================================ */
'use strict';

// ─── SESSION HELPERS ─────────────────────────────────────────
const Session = {
    get() { try { return JSON.parse(sessionStorage.getItem('sl_user')); } catch { return null; } },
    set(u) { sessionStorage.setItem('sl_user', JSON.stringify(u)); },
    clear() { sessionStorage.removeItem('sl_user'); },
};

// ─── MOCK STUDENT (for demo login) ────────────────────────────
const mockStudent = {
    name: 'Ahmed Raza', email: 'ahmed@email.com', phone: '0300-1234567',
    role: 'student', rollNumber: 'SL-2025-0042', branch: 'Gulshan Branch',
    registrationDate: '2025-02-10',
};

// ─── SAMPLE MCQs (for demo exam) ──────────────────────────────
const sampleMCQs = [
    { id: 1, q: 'Which HTML tag creates a hyperlink?', a: '<a>', b: '<link>', c: '<href>', d: '<url>', correct: 'A' },
    { id: 2, q: 'What does CSS stand for?', a: 'Computer Style Sheets', b: 'Colorful Style Sheets', c: 'Cascading Style Sheets', d: 'Creative Style Sheets', correct: 'C' },
    { id: 3, q: 'Which is a JavaScript framework?', a: 'Django', b: 'Laravel', c: 'React', d: 'Flask', correct: 'C' },
    { id: 4, q: 'What does SQL stand for?', a: 'Structured Query Language', b: 'Simple Query Language', c: 'Standard Query Logic', d: 'System Query Language', correct: 'A' },
    { id: 5, q: 'CSS property for background color?', a: 'color', b: 'bg-color', c: 'background-color', d: 'bgcolor', correct: 'C' },
    { id: 6, q: 'Which tag is used for unordered lists in HTML?', a: '<ol>', b: '<ul>', c: '<li>', d: '<list>', correct: 'B' },
    { id: 7, q: 'Which SQL command retrieves data?', a: 'INSERT', b: 'UPDATE', c: 'DELETE', d: 'SELECT', correct: 'D' },
    { id: 8, q: 'What does PHP stand for?', a: 'Personal Home Page', b: 'Preprocessed Hypertext Page', c: 'PHP: Hypertext Preprocessor', d: 'Public HTML Page', correct: 'C' },
];

// ─── UTILS ────────────────────────────────────────────────────
function fmtDate(d) {
    if (!d) return '—';
    return new Date(d).toLocaleDateString('en-PK', { day: 'numeric', month: 'short', year: 'numeric' });
}
function fmtTime(s) {
    return `${String(Math.floor(s / 60)).padStart(2, '0')}:${String(s % 60).padStart(2, '0')}`;
}

// ─── NAVBAR SCROLL + MOBILE TOGGLE ───────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    const nav = document.getElementById('navbar');
    if (nav) {
        window.addEventListener('scroll', () => nav.classList.toggle('scrolled', window.scrollY > 20), { passive: true });
        if (window.scrollY > 20) nav.classList.add('scrolled');
    }
    const toggle = document.getElementById('navToggle');
    const mNav = document.getElementById('mobileNav');
    if (toggle && mNav) {
        toggle.addEventListener('click', () => {
            toggle.classList.toggle('open');
            mNav.classList.toggle('open');
        });
        mNav.querySelectorAll('a').forEach(a => a.addEventListener('click', () => {
            toggle.classList.remove('open');
            mNav.classList.remove('open');
        }));
    }

    // Update navbar auth buttons based on session
    // updateNavAuth();

    // Counter animation
    initCounters();

    // FAQ
    initFAQ();

    // Portal page redirect if already logged in
    if (document.getElementById('portalBox') && Session.get()) {
        window.location.href = 'dashboard.html';
    }

    // Dashboard init
    if (document.getElementById('dashMain')) {
        initDashboard();
    }
});

// ─── NAVBAR AUTH BUTTONS ──────────────────────────────────────
// function updateNavAuth() {
//   const user = Session.get();
//   const authSlot = document.getElementById('navAuthSlot');
//   const authSlotMobile = document.getElementById('navAuthSlotMobile');
//   const isSubPage = window.location.pathname.includes('/pages/');
//   const p = isSubPage ? '' : 'pages/';

//   if (authSlot) {
//     authSlot.innerHTML = user
//       ? `<a href="${p}dashboard.html" class="btn btn-outline btn-sm"><i class="bi bi-grid"></i> ${user.name.split(' ')[0]}'s Portal</a>
//          <button onclick="logoutUser()" class="btn btn-ghost btn-sm"><i class="bi bi-box-arrow-right"></i></button>`
//         : `<a href="${p}portal.html" class="btn btn-gold btn-sm">
//       <i class="bi bi-person-circle"></i> Student Login</a>`;
//   }
//   if (authSlotMobile) {
//     authSlotMobile.innerHTML = user
//       ? `<a href="${p}dashboard.html" class="btn btn-gold"><i class="bi bi-grid"></i> My Portal</a>`
//       : `<a href="${p}portal.html" class="btn btn-gold"><i class="bi bi-person-circle"></i> Student Login</a>`;
//   }
// }

// ─── TOAST ────────────────────────────────────────────────────
function showToast(msg, type = 'info') {
    const c = document.getElementById('toastContainer');
    if (!c) return;
    const icons = { success: 'bi-check-circle-fill', error: 'bi-x-circle-fill', info: 'bi-info-circle-fill' };
    const t = document.createElement('div');
    t.className = `toast ${type}`;
    t.innerHTML = `<i class="bi ${icons[type] || icons.info}"></i><span class="toast-msg">${msg}</span>
    <button class="toast-close" onclick="this.parentElement.remove()"><i class="bi bi-x"></i></button>`;
    c.appendChild(t);
    setTimeout(() => t.remove(), 4500);
}

// ─── LOADING BTN ─────────────────────────────────────────────
function btnLoad(id, on) {
    const b = document.getElementById(id);
    if (!b) return;
    if (on) { b.dataset.orig = b.innerHTML; b.innerHTML = '<span class="loader" style="width:18px;height:18px;border-width:2px;"></span>'; b.disabled = true; }
    else { b.innerHTML = b.dataset.orig || b.innerHTML; b.disabled = false; }
}

// ─── COUNTER ANIMATION ────────────────────────────────────────
function initCounters() {
    document.querySelectorAll('[data-counter]').forEach(el => {
        const target = +el.dataset.counter, suffix = el.dataset.suffix || '';
        let cur = 0; const step = target / 60;
        const run = () => { cur = Math.min(cur + step, target); el.textContent = Math.floor(cur) + suffix; if (cur < target) requestAnimationFrame(run); };
        requestAnimationFrame(run);
    });
}

// ─── FAQ TOGGLE ───────────────────────────────────────────────
function initFAQ() {
    document.querySelectorAll('.faq-item').forEach(item => {
        item.querySelector('.faq-question')?.addEventListener('click', () => {
            const wasOpen = item.classList.contains('open');
            document.querySelectorAll('.faq-item').forEach(f => f.classList.remove('open'));
            if (!wasOpen) item.classList.add('open');
        });
    });
}

// ─── MODAL ────────────────────────────────────────────────────
function openModal(id) { document.getElementById(id)?.classList.add('open'); document.body.style.overflow = 'hidden'; }
function closeModal(id) {
    document.getElementById(id)?.classList.remove('open');
    document.body.style.overflow = '';
    if (id === 'examModal') clearInterval(window._quizTimer);
}
document.addEventListener('click', e => {
    if (e.target.classList.contains('modal-overlay')) closeModal(e.target.id);
});

// ─── PASSWORD TOGGLE ─────────────────────────────────────────
function togglePwd(inputId, btn) {
    const inp = document.getElementById(inputId);
    if (!inp) return;
    const show = inp.type === 'password';
    inp.type = show ? 'text' : 'password';
    btn.querySelector('i').className = `bi bi-eye${show ? '-slash' : ''}`;
}

// ─── LOGIN ────────────────────────────────────────────────────
// function handleLogin(e) {
//   e?.preventDefault();
//   const email = document.getElementById('loginEmail')?.value.trim();
//   const pass  = document.getElementById('loginPass')?.value;
//   if (!email || !pass) { showToast('Please fill in all fields.', 'error'); return; }
//   if (!email.includes('@')) { showToast('Enter a valid email.', 'error'); return; }
//   btnLoad('loginBtn', true);
//   setTimeout(() => {
//     Session.set(mockStudent);
//     showToast(`Welcome back, ${mockStudent.name}!`, 'success');
//     btnLoad('loginBtn', false);
//     setTimeout(() => window.location.href = 'dashboard.html', 700);
//   }, 1200);
// }

// ─── REGISTER ─────────────────────────────────────────────────
// function handleRegister(e) {
//   e?.preventDefault();
//   const n  = document.getElementById('regName')?.value.trim();
//   const em = document.getElementById('regEmail')?.value.trim();
//   const p  = document.getElementById('regPass')?.value;
//   const ph = document.getElementById('regPhone')?.value.trim();
//   if (!n||!em||!p||!ph) { showToast('Please fill in all fields.', 'error'); return; }
//   btnLoad('registerBtn', true);
//   setTimeout(() => {
//     btnLoad('registerBtn', false);
//     showToast('Registration successful! Please login.', 'success');
//     switchPortalTab('login');
//   }, 1200);
// }

// ─── PORTAL TAB SWITCH ────────────────────────────────────────
function switchPortalTab(tab) {
    document.querySelectorAll('.tab-btn').forEach(b => b.classList.toggle('active', b.dataset.tab === tab));
    document.querySelectorAll('.tab-panel').forEach(p => p.classList.toggle('active', p.dataset.tab === tab));
}

// ─── LOGOUT ───────────────────────────────────────────────────
// function logoutUser() {
//   Session.clear();
//   showToast('Logged out successfully.', 'info');
//   const isSubPage = window.location.pathname.includes('/pages/');
//   setTimeout(() => window.location.href = isSubPage ? '../index.html' : 'index.html', 800);
// }

// ─── CONTACT FORM ─────────────────────────────────────────────
function handleContact(e) {
    e?.preventDefault();
    const n = document.getElementById('contactName')?.value.trim();
    const em = document.getElementById('contactEmail')?.value.trim();
    const msg = document.getElementById('contactMsg')?.value.trim();
    if (!n || !em || !msg) { showToast('Please fill required fields.', 'error'); return; }
    btnLoad('contactBtn', true);
    setTimeout(() => {
        btnLoad('contactBtn', false);
        showToast('Message sent! We will reply within 24 hours.', 'success');
        ['contactName', 'contactEmail', 'contactSubject', 'contactMsg'].forEach(id => {
            const el = document.getElementById(id); if (el) el.value = '';
        });
    }, 1000);
}

// ─── COURSE DETAIL MODAL ─────────────────────────────────────
function showCourseModal(id) {
    document.getElementById('modalCourseTitle').textContent = 'Loading…';
    document.getElementById('modalCourseBody').innerHTML = '<p style="text-align:center;color:var(--text-muted);">Loading course details…</p>';
    openModal('courseModal');

    fetch(`/Course/Details/${id}`)
        .then(r => { if (!r.ok) throw new Error('not found'); return r.json(); })
        .then(c => {
            document.getElementById('modalCourseTitle').textContent = c.title;
            document.getElementById('modalCourseBody').innerHTML = `
        <div style="text-align:center;margin-bottom:24px;">
          <div style="font-size:4rem;margin-bottom:10px;">${c.icon}</div>
          <p>${c.desc}</p>
        </div>
        ${c.tracks.map(t => `
          <div style="background:var(--bg-card2);border:1px solid var(--border-light);border-radius:var(--radius-md);padding:20px;margin-bottom:14px;">
            <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:12px;">
              <div><strong style="color:var(--text-primary);">${t.name} Track</strong></div>
              <span style="font-family:var(--font-display);color:var(--gold);">PKR ${Number(t.fee).toLocaleString()}</span>
            </div>
            <div style="font-size:.82rem;color:var(--text-muted);margin-bottom:10px;">
              <i class="bi bi-clock" style="color:var(--gold);margin-right:6px;"></i>${t.duration} Months
            </div>
            ${t.topics.length ? `<div style="display:flex;flex-wrap:wrap;gap:6px;">${t.topics.map(tp => `<span class="badge badge-gold">${tp}</span>`).join('')}</div>` : ''}
          </div>`).join('')}
        <a href="/EntranceExam/enExam" class="btn btn-gold btn-full" style="margin-top:8px;">
          Apply for Entrance Exam <i class="bi bi-arrow-right"></i>
        </a>`;
        })
        .catch(() => {
            document.getElementById('modalCourseTitle').textContent = 'Course Details';
            document.getElementById('modalCourseBody').innerHTML = '<p style="text-align:center;color:var(--text-muted);">Could not load course details.</p>';
        });
}

// ─── QUIZ ENGINE ──────────────────────────────────────────────
const Quiz = {
    state: { qs: [], cur: 0, answers: {}, timeLeft: 0 },

    start(questions, totalSeconds) {
        this.state = { qs: questions, cur: 0, answers: {}, timeLeft: totalSeconds };
        this.render();
        clearInterval(window._quizTimer);
        window._quizTimer = setInterval(() => {
            this.state.timeLeft--;
            const el = document.getElementById('timerNum');
            const wr = document.getElementById('quizTimer');
            if (el) el.textContent = fmtTime(this.state.timeLeft);
            if (wr) wr.classList.toggle('warning', this.state.timeLeft < 60);
            if (this.state.timeLeft <= 0) { clearInterval(window._quizTimer); this.submit(); }
        }, 1000);
    },

    render() {
        const { qs, cur, answers } = this.state;
        const q = qs[cur];
        const el = document.getElementById('quizContent');
        if (!el || !q) return;
        const pct = (cur / qs.length) * 100;
        const sel = answers[q.id];
        el.innerHTML = `
      <div class="quiz-progress-top">
        <span>Question ${cur + 1} of ${qs.length}</span>
        <span id="quizTimer" class="quiz-timer"><i class="bi bi-clock"></i> <span id="timerNum">${fmtTime(this.state.timeLeft)}</span></span>
      </div>
      <div class="progress-bar" style="margin-bottom:24px;"><div class="progress-fill" style="width:${pct}%;"></div></div>
      <div class="quiz-card">
        <div class="quiz-question-text">
          <span style="color:var(--gold);font-family:var(--font-display);font-size:1.2rem;margin-right:8px;">Q${cur + 1}.</span>${q.q}
        </div>
        <div class="quiz-options">
          ${['a', 'b', 'c', 'd'].map(opt => `
            <div class="quiz-option ${sel === opt.toUpperCase() ? 'selected' : ''}" onclick="Quiz.pick('${q.id}','${opt.toUpperCase()}')">
              <div class="option-letter">${opt.toUpperCase()}</div>
              <span>${q[opt]}</span>
            </div>`).join('')}
        </div>
      </div>
      <div style="display:flex;justify-content:space-between;align-items:center;">
        <button class="btn btn-outline btn-sm" onclick="Quiz.nav(-1)" ${cur === 0 ? 'disabled style="opacity:.4;"' : ''}>
          <i class="bi bi-arrow-left"></i> Prev
        </button>
        <span style="font-size:.8rem;color:var(--text-muted);">${Object.keys(answers).length}/${qs.length} answered</span>
        ${cur < qs.length - 1
                ? `<button class="btn btn-ghost btn-sm" onclick="Quiz.nav(1)">Next <i class="bi bi-arrow-right"></i></button>`
                : `<button class="btn btn-gold btn-sm" onclick="Quiz.submit()">Submit <i class="bi bi-check2"></i></button>`}
      </div>`;
    },

    pick(qId, opt) { this.state.answers[qId] = opt; this.render(); },
    nav(d) { this.state.cur += d; this.render(); },

    submit() {
        clearInterval(window._quizTimer);
        const { qs, answers } = this.state;
        const correct = qs.filter(q => answers[q.id] === q.correct).length;
        const pct = Math.round((correct / qs.length) * 100);
        const track = pct >= 50 ? 'Advanced' : 'Basic';
        const offset = 314 - (314 * pct / 100);
        const el = document.getElementById('quizContent');
        if (!el) return;
        el.innerHTML = `
      <div style="text-align:center;padding:20px 0;">
        <h3 style="margin-bottom:4px;">Exam Submitted!</h3>
        <p style="color:var(--text-muted);margin-bottom:28px;">Your results are ready</p>
        <div class="score-ring" style="margin:0 auto 20px;">
          <svg viewBox="0 0 110 110" width="130" height="130">
            <circle class="score-ring-bg" cx="55" cy="55" r="50"/>
            <circle class="score-ring-fill" cx="55" cy="55" r="50"
              style="stroke-dashoffset:${offset};stroke:${pct >= 50 ? 'var(--gold)' : 'var(--info)'};" />
          </svg>
          <div class="score-ring-text">
            <span class="score-percent" style="color:${pct >= 50 ? 'var(--gold)' : 'var(--info)'};">${pct}%</span>
            <span style="font-size:.65rem;color:var(--text-muted);">${correct}/${qs.length}</span>
          </div>
        </div>
        <div style="display:flex;gap:28px;justify-content:center;margin-bottom:24px;">
          <div><div style="font-size:1.6rem;font-weight:700;color:var(--success);">${correct}</div><div style="font-size:.75rem;color:var(--text-muted);">Correct</div></div>
          <div><div style="font-size:1.6rem;font-weight:700;color:var(--danger);">${qs.length - correct}</div><div style="font-size:.75rem;color:var(--text-muted);">Wrong</div></div>
        </div>
        <div style="background:var(--bg-card2);border:1px solid var(--border);border-radius:var(--radius-md);padding:16px 24px;margin-bottom:24px;display:inline-block;">
          <div style="font-size:.78rem;color:var(--text-muted);margin-bottom:4px;">You are assigned to</div>
          <div style="font-family:var(--font-display);font-size:1.5rem;color:${pct >= 50 ? 'var(--gold)' : 'var(--info)'};">${track} Track</div>
          <div style="font-size:.78rem;color:var(--text-muted);margin-top:2px;">${pct >= 50 ? 'CPISM — Advanced Program' : 'DISM — Foundation Program'}</div>
        </div>
        <br><button class="btn btn-gold" onclick="closeModal('examModal')">Close</button>
      </div>`;
    }
};

function startDemoExam() {
    if (!Session.get()) { showToast('Please login first.', 'info'); return; }
    openModal('examModal');
    Quiz.start(sampleMCQs, 480);
}

// ─── EXAM APPLY MODAL ─────────────────────────────────────────
function applyExam(examId, examTitle, examDate, examFee) {
    if (!Session.get()) { showToast('Please login to apply.', 'info'); setTimeout(() => window.location.href = '/signin/login', 800); return; }
    let branchOptions = '';

    branches.forEach(branch => {
        branchOptions += `
        <option value="${branch.id}">
            ${branch.name}
        </option>
    `;
    });
    const body = document.getElementById('applyModalBody');
    if (!body) return;
    body.innerHTML = `
    <div style="background:var(--bg-card2);border:1px solid var(--border-light);border-radius:var(--radius-md);padding:18px;margin-bottom:20px;">
      <div style="font-size:.75rem;color:var(--text-muted);margin-bottom:3px;text-transform:uppercase;">Exam</div>
      <strong>${examTitle}</strong>
      <div class="exam-meta" style="margin-top:8px;">
        <span class="exam-meta-item"><i class="bi bi-calendar3"></i>${examDate}</span>
        <span class="exam-meta-item"><i class="bi bi-currency-rupee"></i>PKR ${examFee}</span>
      </div>
    </div>
    <div class="form-group"><label class="form-label">Preferred Branch</label>
     <select id="branchId" class="form-control">
    ${branchOptions}
</select></div>
    <div class="form-group"><label class="form-label">Payment Method</label>
      <select id="paymentMethod" class="form-control"><option>Cash</option><option>Cheque</option><option>Draft</option></select></div>
    <div style="background:var(--gold-dim);border:1px solid var(--border);border-radius:var(--radius-sm);padding:12px 14px;font-size:.8rem;color:var(--text-secondary);">
      <i class="bi bi-info-circle" style="color:var(--gold);margin-right:5px;"></i>
      Pay at your selected branch. Bring roll number and receipt on exam day.
    </div>`;
    document.getElementById('applyModalTitle').textContent = `Apply — ${examTitle}`;
    document.getElementById('confirmApplyBtn').onclick = () => {
        const branchId = document.getElementById('branchId')?.value;
        const paymentMethod = document.getElementById('paymentMethod')?.value || 'Cash';
        if (!branchId) { showToast('Please select a branch.', 'error'); return; }
        btnLoad('confirmApplyBtn', true);
        fetch('/EntranceExam/Apply', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `examId=${encodeURIComponent(examId)}&branchId=${encodeURIComponent(branchId)}&paymentMethod=${encodeURIComponent(paymentMethod)}`
        })
            .then(r => r.json())
            .then(data => {
                btnLoad('confirmApplyBtn', false);
                closeModal('applyModal');
                if (data.success) {
                    showToast('Application submitted! You now have full dashboard access.', 'success');
                          setTimeout(() => window.location.reload(), 900);
                } else {
                    showToast(data.message || 'Could not submit application.', 'info');
                }
            })
            .catch(() => { btnLoad('confirmApplyBtn', false); closeModal('applyModal'); showToast('Network error — try again.', 'info'); });
    };
    openModal('applyModal');
}

// ─── DASHBOARD ────────────────────────────────────────────────
function initDashboard() {
    const user = Session.get();
    if (!user) { window.location.href = 'portal.html'; return; }

    // Populate sidebar
    const av = document.getElementById('sidebarAvatar');
    const nm = document.getElementById('sidebarName');
    const rl = document.getElementById('sidebarRoll');
    if (av) av.textContent = user.name.charAt(0);
    if (nm) nm.textContent = user.name;
    if (rl) rl.textContent = user.rollNumber || 'Student';

    renderDashTab('overview');

    document.querySelectorAll('.sidebar-link[data-tab]').forEach(l =>
        l.addEventListener('click', () => renderDashTab(l.dataset.tab))
    );

    // Mobile sidebar
    function checkMobileBtn() {
        const btn = document.getElementById('mobileSidebarBtn');
        if (btn) btn.style.display = window.innerWidth <= 1024 ? 'flex' : 'none';
    }
    window.addEventListener('resize', checkMobileBtn);
    checkMobileBtn();
}

function toggleMobileSidebar() {
    const sidebar = document.querySelector('.sidebar');
    if (!sidebar) return;
    const isOpen = sidebar.style.display === 'flex';
    sidebar.style.display = isOpen ? 'none' : 'flex';
    sidebar.style.position = 'fixed';
    sidebar.style.top = '72px';
    sidebar.style.left = '0';
    sidebar.style.bottom = '0';
    sidebar.style.zIndex = '400';
    sidebar.style.width = '260px';
    sidebar.style.flexDirection = 'column';
}

function renderDashTab(tab) {
    document.querySelectorAll('.sidebar-link').forEach(l => l.classList.toggle('active', l.dataset.tab === tab));
    const main = document.getElementById('dashMain');
    if (!main) return;
    const s = Session.get() || mockStudent;

    // const enrollments = [
    //   { course:'Web Development', track:'Advanced', date:'2025-03-01', status:'Active', icon:'🌐' },
    // ];
    // const entranceResults = [
    //   { exam:'Web Dev Entrance Test', marks:14, total:20, percentage:70, track:'Advanced', status:'pass', date:'2025-02-20' },
    // ];
    //   const payments = [
    //       { type: examFee, amount: Amount, method: paymen, date: da, ref: 'ENT-001' },
    //   { type:'CourseFee',   amount:15000, method:'Cheque', date:'2025-03-01', ref:'CHQ-2024' },
    // ];
    // const labRegistrations = [
    //   { session:'During Course Lab', course:'Web Development', date:'2025-03-05', fee:0 },
    // ];

    const renders = {
        overview: () => `
      <div class="dash-welcome">
        <div style="position:relative;z-index:1;">
          <p style="font-size:.78rem;color:var(--text-muted);text-transform:uppercase;letter-spacing:.08em;margin-bottom:4px;">Student Portal</p>
          <h2 style="margin-bottom:6px;">Welcome back, <span style="color:var(--gold);">${s.name}</span></h2>
          <p style="font-size:.875rem;margin-bottom:20px;">
            Roll No: <strong style="color:var(--gold);">${s.rollNumber}</strong>
            &nbsp;•&nbsp; ${s.branch}
          </p>
          <div style="display:flex;gap:12px;flex-wrap:wrap;">
            <button onclick="renderDashTab('results')" class="btn btn-gold btn-sm"><i class="bi bi-bar-chart"></i> My Results</button>
            <button onclick="renderDashTab('payments')" class="btn btn-outline btn-sm"><i class="bi bi-receipt"></i> Payments</button>
            <button onclick="startDemoExam()" class="btn btn-ghost btn-sm"><i class="bi bi-pencil-square"></i> Take Demo Exam</button>
          </div>
        </div>
      </div>
      <div class="stat-grid">
        <div class="stat-card"><div class="stat-icon gold"><i class="bi bi-book"></i></div>
          <div class="stat-num">${enrollments.length}</div><div class="stat-label">Courses Enrolled</div></div>
        <div class="stat-card"><div class="stat-icon green"><i class="bi bi-patch-check"></i></div>
          <div class="stat-num">${entranceResults.length}</div><div class="stat-label">Exams Taken</div></div>
        <div class="stat-card"><div class="stat-icon blue"><i class="bi bi-wallet2"></i></div>
          <div class="stat-num">${payments.length}</div><div class="stat-label">Payments Made</div></div>
        <div class="stat-card"><div class="stat-icon red"><i class="bi bi-trophy"></i></div>
          <div class="stat-num">${entranceResults[0]?.percentage}%</div><div class="stat-label">Best Score</div></div>
      </div>
      <div class="grid-2">
        <div class="table-wrapper">
          <div class="table-header"><h4>My Enrollments</h4></div>
          <table><thead><tr><th>Course</th><th>Track</th><th>Status</th></tr></thead>
          <tbody>${enrollments.map(e => `<tr><td><strong>${e.course}</strong></td>
            <td><span class="badge badge-gold">${e.track}</span></td>
            <td><span class="badge badge-green">${e.status}</span></td></tr>`).join('')}</tbody></table>
        </div>
        <div class="table-wrapper">
          <div class="table-header"><h4>Recent Payments</h4></div>
          <table><thead><tr><th>Type</th><th>Amount</th><th>Method</th></tr></thead>
          <tbody>${payments.map(p => `<tr><td>${p.type}</td>
            <td><strong>PKR ${p.amount.toLocaleString()}</strong></td>
            <td><span class="badge badge-gold">${p.method}</span></td></tr>`).join('')}</tbody></table>
        </div>
      </div>`,

        enrollments: () => `
      <h3 style="margin-bottom:24px;">My <span style="color:var(--gold);">Enrollments</span></h3>
      <div class="grid-2">
        ${enrollments.map(e => `
          <div class="card card-body" style="border-radius:var(--radius-lg);">
            <div style="display:flex;align-items:center;gap:14px;margin-bottom:18px;">
              <div class="course-icon">${e.icon}</div>
              <div><h4>${e.course}</h4><span class="badge badge-gold">${e.track} Track</span></div>
            </div>
            <p style="font-size:.85rem;"><i class="bi bi-calendar3" style="color:var(--gold);margin-right:6px;"></i>Enrolled: ${fmtDate(e.date)}</p>
            <p style="font-size:.85rem;margin-top:6px;"><i class="bi bi-building" style="color:var(--gold);margin-right:6px;"></i>${s.branch}</p>
            <div style="margin-top:14px;padding-top:14px;border-top:1px solid var(--border-light);">
              <span class="badge badge-green">${e.status}</span>
            </div>
          </div>`).join('')}
        <div style="background:var(--bg-card);border:2px dashed var(--border-light);border-radius:var(--radius-lg);display:flex;flex-direction:column;align-items:center;justify-content:center;gap:12px;min-height:180px;padding:24px;text-align:center;">
          <i class="bi bi-plus-circle" style="font-size:2rem;color:var(--text-muted);"></i>
          <p style="font-size:.875rem;">Apply for another course</p>
          <a href="entrance-exams.html" class="btn btn-gold btn-sm">View Exams</a>
        </div>
      </div>`,

        results: () => `
      <h3 style="margin-bottom:24px;">Entrance <span style="color:var(--gold);">Results</span></h3>
      ${entranceResults.map(r => {
            const off = 314 - (314 * r.percentage / 100);
            return `<div class="result-card" style="margin-bottom:20px;">
          <div class="result-header">
            <div><h4>${r.exam}</h4><p style="font-size:.82rem;margin-top:4px;">Date: ${fmtDate(r.date)}</p></div>
            <span class="badge badge-${r.status === 'pass' ? 'green' : 'red'}">${r.status.toUpperCase()}</span>
          </div>
          <div class="result-body" style="display:grid;grid-template-columns:auto 1fr;gap:32px;align-items:center;">
            <div style="text-align:center;">
              <div class="score-ring">
                <svg viewBox="0 0 110 110" width="120" height="120">
                  <circle class="score-ring-bg" cx="55" cy="55" r="50"/>
                  <circle class="score-ring-fill" cx="55" cy="55" r="50" style="stroke-dashoffset:${off};"/>
                </svg>
                <div class="score-ring-text">
                  <span class="score-percent">${r.percentage}%</span>
                  <span style="font-size:.65rem;color:var(--text-muted);">${r.marks}/${r.total}</span>
                </div>
              </div>
            </div>
            <div>
              <div style="margin-bottom:14px;">
                <div style="font-size:.8rem;color:var(--text-muted);margin-bottom:6px;">Score Progress</div>
                <div class="progress-bar"><div class="progress-fill" style="width:${r.percentage}%;"></div></div>
              </div>
              <div style="background:var(--bg-card2);border:1px solid var(--border);border-radius:var(--radius-md);padding:14px 18px;display:inline-flex;gap:8px;align-items:center;">
                <i class="bi bi-bookmark-fill" style="color:var(--gold);"></i>
                <span style="font-size:.88rem;">Assigned Track: <strong style="color:var(--gold);">${r.track}</strong></span>
              </div>
            </div>
          </div>
        </div>`;
        }).join('')}`,

        lab: () => `
      <h3 style="margin-bottom:24px;">Lab <span style="color:var(--gold);">Sessions</span></h3>
      <div class="table-wrapper">
        <div class="table-header"><h4>My Lab Registrations</h4></div>
        <table><thead><tr><th>Session</th><th>Course</th><th>Registered On</th><th>Fee</th></tr></thead>
        <tbody>${labRegistrations.map(l => `<tr>
          <td><strong>${l.session}</strong></td><td>${l.course}</td>
          <td>${fmtDate(l.date)}</td>
        <td>
              ${l.fee === 0
                ? `<span class="badge badge-green">Free</span>`
                : `PKR ${l.fee}`}
            </td>
          </tr>
        `).join('')}
      </tbody>
    </table>
  </div>
`, 

        payments: () => `
      <h3 style="margin-bottom:24px;">Payment <span style="color:var(--gold);">History</span></h3>
      <div class="table-wrapper">
        <div class="table-header">
          <h4>All Transactions</h4>
          <span style="font-size:.8rem;color:var(--text-muted);">Roll: ${s.rollNumber}</span>
        </div>
        <table><thead><tr><th>#</th><th>Type</th><th>Amount</th><th>Method</th><th>Ref No.</th><th>Date</th></tr></thead>
        <tbody>${payments.map((p, i) => `<tr>
          <td>${i + 1}</td><td><strong>${p.type}</strong></td>
          <td><strong style="color:var(--success);">PKR ${p.amount.toLocaleString()}</strong></td>
          <td><span class="badge badge-gold">${p.method}</span></td>
          <td style="color:var(--text-muted);">${p.ref || '—'}</td>
          <td>${fmtDate(p.date)}</td></tr>`).join('')}</tbody></table>
      </div>
      <div style="background:var(--bg-card);border:1px solid var(--border-light);border-radius:var(--radius-md);padding:18px 24px;margin-top:14px;display:flex;align-items:center;justify-content:space-between;">
        <span style="font-size:.9rem;color:var(--text-secondary);">Total Paid</span>
        <span style="font-family:var(--font-display);font-size:1.5rem;color:var(--gold);">PKR ${payments.reduce((a, p) => a + p.amount, 0).toLocaleString()}</span>
      </div>`,

        profile: () => `
      <h3 style="margin-bottom:24px;">My <span style="color:var(--gold);">Profile</span></h3>
      <div style="max-width:560px;">
        <div class="dash-welcome" style="margin-bottom:24px;display:flex;align-items:center;gap:22px;">
          <div style="width:70px;height:70px;border-radius:50%;background:linear-gradient(135deg,var(--gold),#8b6914);display:flex;align-items:center;justify-content:center;font-family:var(--font-display);font-size:1.8rem;font-weight:700;color:var(--bg-dark);flex-shrink:0;position:relative;z-index:1;">${s.name.charAt(0)}</div>
          <div style="position:relative;z-index:1;"><h3 style="margin-bottom:4px;">${s.name}</h3>
          <p style="font-size:.85rem;">${s.email}</p>
          <span class="badge badge-gold" style="margin-top:6px;">Student</span></div>
        </div>
        <div class="table-wrapper">
          ${[['Full Name', s.name], ['Email', s.email], ['Phone', s.phone || '—'], ['Roll Number', s.rollNumber],
            ['Branch', s.branch], ['Registered', fmtDate(s.registrationDate)], ['Role', 'Student']]
                .map(([k, v]) => `<div style="display:flex;justify-content:space-between;padding:13px 20px;border-bottom:1px solid var(--border-light);">
              <span style="font-size:.8rem;color:var(--text-muted);font-weight:600;text-transform:uppercase;letter-spacing:.06em;">${k}</span>
              <span style="font-size:.875rem;color:var(--text-primary);">${v}</span></div>`).join('')}
        </div>
        <div style="display:flex;gap:12px;margin-top:20px;">
          <button class="btn btn-outline" onclick="showToast('Profile editing coming soon!','info')"><i class="bi bi-pencil"></i> Edit Profile</button>
          <button class="btn btn-ghost" onclick="logoutUser()"><i class="bi bi-box-arrow-right"></i> Logout</button>
        </div>
      </div>`,
    };
    main.innerHTML = `<div class="page-fade">${(renders[tab] || renders.overview)()}</div>`;
}

// ===== Course page: track filter (Basic / Advanced / All) =====
function filterCourses(filter) {
    document.querySelectorAll('.filter-btn').forEach(btn => {
        const active = btn.dataset.filter === filter;
        btn.classList.toggle('btn-gold', active);
        btn.classList.toggle('btn-ghost', !active);
    });

    document.querySelectorAll('#coursesGrid .card').forEach(card => {
        const rows = card.querySelectorAll('.track-row');
        let visibleCount = 0;
        rows.forEach(row => {
            const show = filter === 'all' || row.dataset.track === filter;
            row.style.display = show ? 'flex' : 'none';
            if (show) visibleCount++;
        });
        card.style.display = (filter === 'all' || visibleCount > 0) ? '' : 'none';
    });
}
