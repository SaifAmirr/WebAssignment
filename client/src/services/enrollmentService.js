import api from './api';

export const createEnrollment = (data) => api.post('/enrollments', data);
export const getStudentEnrollments = (studentId) => api.get(`/enrollments/student/${studentId}`);
export const getCourseEnrollments = (courseId) => api.get(`/enrollments/course/${courseId}`);
export const updateGrade = (studentId, courseId, data) =>
  api.put(`/enrollments/${studentId}/courses/${courseId}`, data);
