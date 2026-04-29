db = db.getSiblingDB('admin');

const databases = [
  { name: 'learnflow_identity', user: 'identity_user' },
  { name: 'learnflow_courses', user: 'courses_user' },
  { name: 'learnflow_enrollments', user: 'enrollments_user' },
  { name: 'learnflow_progress', user: 'progress_user' },
  { name: 'learnflow_certificates', user: 'certificates_user' },
];

const password = 'service_password_123';

databases.forEach(({ name, user }) => {
  db = db.getSiblingDB(name);
  db.createUser({
    user: user,
    pwd: password,
    roles: [{ role: 'readWrite', db: name }]
  });
  db.createCollection('_init');
  print(`Created user ${user} for database ${name}`);
});